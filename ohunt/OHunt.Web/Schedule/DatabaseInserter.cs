using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using AngleSharp.Common;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OHunt.Web.Database;
using OHunt.Web.Options;

namespace OHunt.Web.Schedule
{
    /// <summary>
    /// Act as a consumer that reads records and write them to the database.
    /// </summary>
    public class DatabaseInserter<TEntity>
        where TEntity : class
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<DatabaseInserter<TEntity>> _logger;
        private readonly IOptions<DatabaseInserterOptions> _options;

        public DatabaseInserter(
            IServiceProvider serviceProvider,
            ILogger<DatabaseInserter<TEntity>> logger,
            IOptions<DatabaseInserterOptions> options)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
            _options = options;
        }

        public async Task WorkAsync(ISourceBlock<TEntity> source, CancellationToken c)
        {
            var bufferSize = _options
                .Value.BufferSize.GetOrDefault(typeof(TEntity).Name, _options.Value.DefaultBufferSize);
            var buffer = new TEntity[bufferSize];

            _logger.LogInformation("Start working, buffer size: {0}", bufferSize);

            var i = 0;
            try
            {
                while (await source.OutputAvailableAsync(c))
                {
                    buffer[i] = await source.ReceiveAsync(c);
                    if (++i >= bufferSize)
                    {
                        await Insert(buffer.Take(i));
                        i = 0;
                    }
                }
            }
            finally
            {
                if (i > 0)
                {
                    await Insert(buffer.Take(i));
                }
            }
        }

        private async Task Insert(IEnumerable<TEntity> submissions)
        {
            _logger.LogTrace("Try to insert records to database");
            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<OHuntDbContext>();
            context.ChangeTracker.AutoDetectChangesEnabled = false;

            await context.Set<TEntity>().AddRangeAsync(submissions);

            context.ChangeTracker.DetectChanges();
            var inserted = await context.SaveChangesAsync();

            _logger.LogInformation("{0} rows inserted", inserted);
        }
    }
}
