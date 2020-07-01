using System;
using System.Linq;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OHunt.Web.Crawlers;
using OHunt.Web.Database;
using OHunt.Web.Models;

namespace OHunt.Web.Schedule
{
    public class SubmissionCrawlerCoordinator
    {
        private const int BufferCapacity = 1000;

        private readonly SubmissionInserter _inserter;
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<SubmissionCrawlerCoordinator> _logger;

        public SubmissionCrawlerCoordinator(
            SubmissionInserter inserter,
            IServiceProvider serviceProvider,
            ILogger<SubmissionCrawlerCoordinator> logger)
        {
            _inserter = inserter;
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        public async Task WorkAsync(ISubmissionCrawler crawler)
        {
            var oj = crawler.OnlineJudge;

            long? latestSubmissionId;
            using (var scope = _serviceProvider.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<OHuntWebContext>();
                latestSubmissionId = (await context.Submission
                    .Where(e => e.OnlineJudgeId == oj)
                    .OrderByDescending(e => e.SubmissionId)
                    .FirstOrDefaultAsync())?.SubmissionId;
            }

            var submissionBuffer
                = new BufferBlock<Submission>(new DataflowBlockOptions
                {
                    BoundedCapacity = BufferCapacity,
                    EnsureOrdered = false,
                });

            _logger.LogTrace("Work on {0}, latestSubmissionId {1}", oj.ToString(), latestSubmissionId);

            var crawlerTask = crawler.Work(latestSubmissionId, submissionBuffer);
            var inserterTask = _inserter.WorkAsync(submissionBuffer);

            try
            {
                await Task.WhenAll(crawlerTask, inserterTask, submissionBuffer.Completion);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Exception when crawling");
            }
        }
    }
}
