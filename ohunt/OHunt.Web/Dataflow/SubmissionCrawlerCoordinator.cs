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
            throw new NotImplementedException("Complete pipeline");

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
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Exception when running crawler {oj.ToString()}");
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

        public async Task WorkAsync(
            ISubmissionCrawler crawler,
            CancellationToken cancellationToken)
        {
            var submissionBuffer
                = new BufferBlock<Submission>(new DataflowBlockOptions
                {
                    BoundedCapacity = BufferCapacity,
                    EnsureOrdered = false,
                });
            var errorBuffer
                = new BufferBlock<CrawlerError>(new DataflowBlockOptions
                {
                    BoundedCapacity = BufferCapacity,
                    EnsureOrdered = false,
                });


            var inserterCancel = new CancellationTokenSource();
            var crawlerCancel = new CancellationTokenSource();

            // cancel crawler, it may trigger crawler to submit a Complete
            // or it just throws, the catch below cancels the inserter
            cancellationToken.Register(() => { crawlerCancel.Cancel(); });

            throw new NotImplementedException();
            // var crawlerTask = crawler.WorkAsync(latestSubmissionId, submissionBuffer, errorBuffer, crawlerCancel.Token);
            // var submissionInserterTask = _submissionInserter.WorkAsync(submissionBuffer, inserterCancel.Token);
            // var errorInserterTask = _errorInserter.WorkAsync(errorBuffer, inserterCancel.Token);
            //
            // try
            // {
            //     await crawlerTask;
            //     await submissionInserterTask;
            //     await errorInserterTask;
            // }
            // catch (Exception e)
            // {
            //     inserterCancel.Cancel();
            //     _logger.LogError(e, "Exception when crawling");
            // }
        }
    }
}
