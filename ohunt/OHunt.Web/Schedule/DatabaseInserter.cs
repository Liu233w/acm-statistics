using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using AngleSharp.Common;
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
        private readonly TEntity[] _buffer;
        private readonly int _bufferSize;

        private int _idx = 0;

        public DatabaseInserter(
            IServiceProvider serviceProvider,
            ILogger<DatabaseInserter<TEntity>> logger,
            IOptions<DatabaseInserterOptions> options)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;

            Target
                = new ActionBlock<DatabaseInserterMessage<TEntity>>(OnReceive, new ExecutionDataflowBlockOptions
                {
                    BoundedCapacity = 0,
                    EnsureOrdered = false,
                    MaxDegreeOfParallelism = 1,
                });

            _bufferSize = options
                .Value.BufferSize.GetOrDefault(typeof(TEntity).Name, options.Value.DefaultBufferSize);
            _buffer = new TEntity[_bufferSize];

            _logger.LogInformation("Initialized, buffer size: {0}", _bufferSize);
        }

        /// <summary>
        /// The block that connected to the inserter
        /// </summary>
        public ActionBlock<DatabaseInserterMessage<TEntity>> Target { get; }

        private async Task OnReceive(DatabaseInserterMessage<TEntity> message)
        {
            if (message.Entity != null)
            {
                _buffer[_idx] = message.Entity;
                ++_idx;
            }

            if (_idx >= _bufferSize || message.ForceInsert)
            {
                await Insert(_buffer.Take(_idx));
                _idx = 0;
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
