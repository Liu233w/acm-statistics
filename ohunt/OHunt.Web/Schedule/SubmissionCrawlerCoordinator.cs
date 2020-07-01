using System;
using System.Linq;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
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

        public SubmissionCrawlerCoordinator(SubmissionInserter inserter, IServiceProvider serviceProvider)
        {
            _inserter = inserter;
            _serviceProvider = serviceProvider;
        }

        public async Task Work(ISubmissionCrawler crawler, OnlineJudge oj)
        {
            long latestSubmissionId;
            using (var scope = _serviceProvider.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<OHuntWebContext>();
                latestSubmissionId = (await context.Submission
                    .Where(e => e.OnlineJudgeId == oj)
                    .OrderByDescending(e => e.SubmissionId)
                    .FirstOrDefaultAsync())?.SubmissionId ?? 0;
            }

            var submissionBuffer
                = new BufferBlock<Submission>(new DataflowBlockOptions
                {
                    BoundedCapacity = BufferCapacity,
                    EnsureOrdered = false,
                });

            var crawlerTask = crawler.Work(latestSubmissionId, submissionBuffer);
            var inserterTask = _inserter.Work(submissionBuffer);

            await Task.WhenAll(crawlerTask, inserterTask);
        }
    }
}
