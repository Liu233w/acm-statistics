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

        private bool _initialized = false;
        private ISubmissionCrawler[] _crawlers = null!;
        private ITargetBlock<CrawlerMessage>[] _targets = null!;
        private Task?[] _crawlerTasks = null!;

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
            _crawlerTasks = new Task[_crawlers.Length];

            _initialized = true;
        }

        /// <summary>
        /// Start all crawlers. If a crawler is not done yet, do nothing.
        /// </summary>
        public void StartAllCrawlers()
        {
            throw new NotImplementedException();
        }

        public async Task WorkAsync(
            ISubmissionCrawler crawler,
            CancellationToken cancellationToken)
        {
            var oj = crawler.OnlineJudge;

            long? latestSubmissionId;
            using (var scope = _serviceProvider.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<OHuntDbContext>();
                latestSubmissionId = (await context.Submission
                    .Where(e => e.OnlineJudgeId == oj)
                    .OrderByDescending(e => e.SubmissionId)
                    .FirstOrDefaultAsync(cancellationToken: cancellationToken))?.SubmissionId;
            }

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

            _logger.LogTrace("Work on {0}, latestSubmissionId {1}", oj.ToString(), latestSubmissionId);

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
