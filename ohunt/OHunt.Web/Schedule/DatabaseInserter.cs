using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OHunt.Web.Database;

namespace OHunt.Web.Schedule
{
    /// <summary>
    /// Act as a consumer that reads records and write them to the database.
    /// </summary>
    public class DatabaseInserter<TEntity>
        where TEntity : class
    {
        private const int DefaultBufferSize = 10000;

        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<DatabaseInserter<TEntity>> _logger;
        private readonly IConfiguration _configuration;

        public DatabaseInserter(
            IServiceProvider serviceProvider,
            ILogger<DatabaseInserter<TEntity>> logger,
            IConfiguration configuration)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
            _configuration = configuration;
        }

        public async Task WorkAsync(ISourceBlock<TEntity> source, CancellationToken c)
        {
            var bufferSize = _configuration.GetSection("DatabaseInserterBufferSize")
                .GetValue(typeof(TEntity).Name, DefaultBufferSize);
            var buffer = new TEntity[bufferSize];

            _logger.LogInformation("Start working, buffer size: {0}", bufferSize);

            while (await source.OutputAvailableAsync(c))
            {
                var i = 0;
                for (;
                    i < bufferSize && await source.OutputAvailableAsync(c);
                    i++)
                {
                    buffer[i] = await source.ReceiveAsync(c);
                }

                await Insert(buffer.Take(i));
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
