using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OHunt.Web.Crawlers;
using OHunt.Web.Database;
using OHunt.Web.Models;

namespace OHunt.Web.Dataflow
{
    public class SubmissionCrawlerCoordinator
    {
        private const int BufferCapacity = 1000;

        private readonly DatabaseInserterFactory _databaseInserterFactory;
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<SubmissionCrawlerCoordinator> _logger;

        private readonly CancellationTokenSource _cancel
            = new CancellationTokenSource();

        private readonly object _lock = new object();

        private bool _initialized = false;
        private ISubmissionCrawler[] _crawlers = null!;
        private ITargetBlock<CrawlerMessage>[] _targets = null!;
        private Task[] _crawlerTasks = null!;
        private DatabaseInserter<Submission> _submissionInserter = null!;
        private DatabaseInserter<CrawlerError> _errorInserter = null!;

        public SubmissionCrawlerCoordinator(
            DatabaseInserterFactory databaseInserterFactory,
            IServiceProvider serviceProvider,
            ILogger<SubmissionCrawlerCoordinator> logger)
        {
            _databaseInserterFactory = databaseInserterFactory;
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        /// <summary>
        /// Initialize using the crawlers
        /// </summary>
        /// <param name="crawlers"></param>
        /// <exception cref="InvalidOperationException"></exception>
        public void Initialize(IEnumerable<ISubmissionCrawler> crawlers)
        {
            if (_initialized)
            {
                throw new InvalidOperationException("Coordinator is initialized");
            }

            _crawlers = crawlers.ToArray();
            _crawlerTasks = Enumerable.Repeat(Task.CompletedTask, _crawlers.Length)
                .ToArray();

            /*
             * Pipeline:
             *              Submission
             *     crawler -----------------> cacher -> submission merger (shared) -> submission inserter (shared)
             *             \
             *              \ CrawlerError
             *               ---------------> cacher ------> error merger (shared) -> error inserter (shared)
             *                     |                                               |
             *                 transform A                                    transform B
             *       (CrawlerMessage -> DataCachingMessage)         (Entity -> DatabaseInserterMessage)
             *
             * For each merger:
             *
             *     crawler 1 -----> MergeBlock -----> DatabaseInserter
             *                 ↑
             *     crawler 2  ↗|
             *                 |
             *     crawler 3  ↗
             */

            var submissionBlocks =
                new IPropagatorBlock<DataCachingMessage<Submission>, Submission>[_crawlers.Length];
            var errorBlocks =
                new IPropagatorBlock<DataCachingMessage<CrawlerError>, CrawlerError>[_crawlers.Length];

            for (int i = 0; i < _crawlers.Length; i++)
            {
                var target = new BufferBlock<CrawlerMessage>(new DataflowBlockOptions());
                _targets[i] = target;

                var submissionTransformA =
                    new TransformBlock<CrawlerMessage, DataCachingMessage<Submission>>(SubmissionTransform);
                var errorTransformA =
                    new TransformBlock<CrawlerMessage, DataCachingMessage<CrawlerError>>(ErrorTransform);

                target.LinkTo(submissionTransformA, new DataflowLinkOptions { PropagateCompletion = true },
                    message => message.Submission != null);
                target.LinkTo(errorTransformA, new DataflowLinkOptions { PropagateCompletion = true },
                    message => message.CrawlerError != null);

                submissionBlocks[i] = DataCachingBlockFactory.CreateBlock<Submission>(BufferCapacity);
                errorBlocks[i] = DataCachingBlockFactory.CreateBlock<CrawlerError>(BufferCapacity);

                submissionTransformA.LinkTo(submissionBlocks[i],
                    new DataflowLinkOptions { PropagateCompletion = true });
                errorTransformA.LinkTo(errorBlocks[i], new DataflowLinkOptions { PropagateCompletion = true });
            }

            var submissionMerger = new MergeBlock<Submission>(submissionBlocks);
            var errorMerger = new MergeBlock<CrawlerError>(errorBlocks);

            var submissionTransformB = CreateInserterMessageTransformer<Submission>();
            var errorTransformB = CreateInserterMessageTransformer<CrawlerError>();
            submissionMerger.LinkTo(submissionTransformB,
                new DataflowLinkOptions { PropagateCompletion = true });
            errorMerger.LinkTo(errorTransformB,
                new DataflowLinkOptions { PropagateCompletion = true });

            _submissionInserter = _databaseInserterFactory.CreateInstance<Submission>();
            _errorInserter = _databaseInserterFactory.CreateInstance<CrawlerError>();
            submissionTransformB.LinkTo(_submissionInserter,
                new DataflowLinkOptions { PropagateCompletion = true });
            errorTransformB.LinkTo(_errorInserter,
                new DataflowLinkOptions { PropagateCompletion = true });

            _initialized = true;
            return;

            static TransformBlock<T, DatabaseInserterMessage<T>>
                CreateInserterMessageTransformer<T>()
                where T : class
            {
                return new TransformBlock<T, DatabaseInserterMessage<T>>(
                    e => DatabaseInserterMessage<T>.OfEntity(e));
            }
        }

        /// <summary>
        /// Start all crawlers. If a crawler is not done yet, do nothing.
        /// </summary>
        public void StartAllCrawlers()
        {
            if (!_initialized)
            {
                throw new InvalidOperationException($"{nameof(SubmissionCrawlerCoordinator)} is not initialized");
            }

            lock (_lock)
            {
                for (int i = 0; i < _crawlers.Length; i++)
                {
                    var crawler = _crawlers[i];
                    if (_crawlerTasks[i].IsCompleted)
                    {
                        _crawlerTasks[i] = StartCrawler(crawler, _targets[i]);
                    }
                    else
                    {
                        _logger.LogInformation(
                            $"Previous crawler {crawler.OnlineJudge.ToString()} is not finished yet");
                    }
                }
            }
        }

        /// <summary>
        /// Cancel all crawlers. Task is done when all crawlers are completed.
        /// </summary>
        public Task Cancel()
        {
            if (!_initialized)
            {
                throw new InvalidOperationException($"{nameof(SubmissionCrawlerCoordinator)} is not initialized");
            }

            _cancel.Cancel();
            return Task.WhenAll(_crawlerTasks);
        }

        private async Task StartCrawler(ISubmissionCrawler crawler, ITargetBlock<CrawlerMessage> target)
        {
            var oj = crawler.OnlineJudge;

            long? latestSubmissionId;
            using (var scope = _serviceProvider.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<OHuntDbContext>();
                latestSubmissionId = (await context.Submission
                    .Where(e => e.OnlineJudgeId == oj)
                    .OrderByDescending(e => e.SubmissionId)
                    .FirstOrDefaultAsync(_cancel.Token))?.SubmissionId;
            }

            _logger.LogTrace("Work on {0}, latestSubmissionId {1}", oj.ToString(), latestSubmissionId);

            try
            {
                await crawler.WorkAsync(latestSubmissionId, target, _cancel.Token);
                // TODO: call this after all crawler finished or after 30 minutes
                await _submissionInserter.SendAsync(DatabaseInserterMessage<Submission>.ForceInsertMessage);
                await _errorInserter.SendAsync(DatabaseInserterMessage<CrawlerError>.ForceInsertMessage);
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Exception when running crawler {oj.ToString()}");

                await target.SendAsync(new CrawlerMessage
                {
                    IsRevertRequested = true,
                });

                // TODO: add entity CrawlerExecuteLog , save the execution time and result of 
                // the crawler
                using var scope = _serviceProvider.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<OHuntDbContext>();
                await context.CrawlerErrors.AddAsync(new CrawlerError
                {
                    Crawler = oj.ToString(),
                    Data = e.ToString(),
                    Message = e.Message,
                    Time = DateTime.Now,
                });
            }
        }

        private static DataCachingMessage<Submission> SubmissionTransform(CrawlerMessage message)
        {
            if (message.IsRevertRequested)
            {
                return DataCachingMessage<Submission>.DiscardMessage;
            }

            if (message.Submission == null)
            {
                if (message.IsCheckPoint)
                {
                    return DataCachingMessage<Submission>.SubmitMessage;
                }
                else
                {
                    throw new ArgumentException(
                        $"The message should either contain a {nameof(Submission)} or be a checkpoint");
                }
            }
            else
            {
                return DataCachingMessage<Submission>.OfEntity(message.Submission, message.IsCheckPoint);
            }
        }

        private static DataCachingMessage<CrawlerError> ErrorTransform(CrawlerMessage message)
        {
            if (message.IsRevertRequested)
            {
                return DataCachingMessage<CrawlerError>.DiscardMessage;
            }

            if (message.CrawlerError == null)
            {
                if (message.IsCheckPoint)
                {
                    return DataCachingMessage<CrawlerError>.SubmitMessage;
                }
                else
                {
                    throw new ArgumentException(
                        $"The message should either contain a {nameof(CrawlerError)} or be a checkpoint");
                }
            }
            else
            {
                return DataCachingMessage<CrawlerError>.OfEntity(message.CrawlerError, message.IsCheckPoint);
            }
        }
    }
}
