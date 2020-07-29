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
        private readonly DatabaseInserterFactory _databaseInserterFactory;
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<SubmissionCrawlerCoordinator> _logger;

        private readonly object _lock = new object();

        private bool _initialized = false;
        private ISubmissionCrawler[] _crawlers = null!;
        private Task[] _crawlerTasks = null!;

        private CancellationTokenSource _cancel =
            new CancellationTokenSource();

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

            _submissionInserter = _databaseInserterFactory.CreateInstance<Submission>();
            _errorInserter = _databaseInserterFactory.CreateInstance<CrawlerError>();

            _initialized = true;
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
                _cancel = new CancellationTokenSource();
                for (int i = 0; i < _crawlers.Length; i++)
                {
                    var crawler = _crawlers[i];
                    if (_crawlerTasks[i].IsCompleted)
                    {
                        _crawlerTasks[i] = StartCrawler(crawler);
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

            lock (_lock)
            {
                _cancel.Cancel();
                return Task.WhenAll(_crawlerTasks);
            }
        }

        private async Task StartCrawler(ISubmissionCrawler crawler)
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

            var submissionTransformer = CreateTransformer<Submission>();
            var errorTransformer = CreateTransformer<CrawlerError>();

            using var submissionUnlink = submissionTransformer.LinkTo(_submissionInserter);
            using var errorUnlink = errorTransformer.LinkTo(_errorInserter);

            var propagator = new CrawlerPropagator(submissionTransformer, errorTransformer);

            try
            {
                await crawler.WorkAsync(latestSubmissionId, propagator, _cancel.Token);
                await propagator.SendAsync(new CrawlerMessage
                {
                    Checkpoint = true,
                });
                propagator.Complete();
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Exception when running crawler {oj.ToString()}");

                // data from last checkpoint is automatically discarded
                propagator.Complete();

                // TODO: add entity CrawlerExecuteLog , save the execution time and result of 
                // the crawler
            }

            await propagator.Completion;
            await submissionTransformer.Completion;
            await errorTransformer.Completion;

            // TODO: call this after all crawler finished or after 30 minutes
            await _submissionInserter.SendAsync(DatabaseInserterMessage<Submission>.ForceInsertMessage);
            await _errorInserter.SendAsync(DatabaseInserterMessage<CrawlerError>.ForceInsertMessage);
        }

        private static DatabaseInserterMessage<T> EntityToMessage<T>(T entity)
            where T : class
        {
            return DatabaseInserterMessage<T>.OfEntity(entity);
        }

        private static TransformBlock<T, DatabaseInserterMessage<T>>
            CreateTransformer<T>()
            where T : class
        {
            return new TransformBlock<T, DatabaseInserterMessage<T>>(
                (Func<T, DatabaseInserterMessage<T>>) EntityToMessage);
        }
    }
}
