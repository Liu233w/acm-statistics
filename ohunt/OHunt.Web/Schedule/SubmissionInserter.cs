using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OHunt.Web.Database;
using OHunt.Web.Models;

namespace OHunt.Web.Schedule
{
    /// <summary>
    /// Act as a consumer that reads Submission records and write them to the database.
    /// </summary>
    public class SubmissionInserter
    {
        private const int MaxRecordCount = 10000;

        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<SubmissionInserter> _logger;

        public SubmissionInserter(
            IServiceProvider serviceProvider,
            ILogger<SubmissionInserter> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        public async Task WorkAsync(ISourceBlock<Submission> source, CancellationToken c)
        {
            var submissions = new Submission[MaxRecordCount];

            while (await source.OutputAvailableAsync(c))
            {
                var i = 0;
                for (;
                    i < MaxRecordCount && await source.OutputAvailableAsync(c);
                    i++)
                {
                    submissions[i] = await source.ReceiveAsync(c);
                }

                await Insert(submissions.Take(i));
            }
        }

        private async Task Insert(IEnumerable<Submission> submissions)
        {
            _logger.LogTrace("Try to insert records to database");
            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<OHuntWebContext>();
            context.ChangeTracker.AutoDetectChangesEnabled = false;

            await context.Submission.AddRangeAsync(submissions);

            context.ChangeTracker.DetectChanges();
            var inserted = await context.SaveChangesAsync();

            _logger.LogTrace("{0} rows inserted", inserted);
        }
    }
}
