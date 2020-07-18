﻿using System;
using System.Collections.Generic;
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

        public DatabaseInserter(
            IServiceProvider serviceProvider,
            ILogger<DatabaseInserter<TEntity>> logger,
            IOptions<DatabaseInserterOptions> options)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;


            var bufferSize = options
                .Value.BufferSize.GetOrDefault(typeof(TEntity).Name, options.Value.DefaultBufferSize);
            Target = new BatchBlock<TEntity>(bufferSize);
            _logger.LogInformation("Initialized, buffer size: {0}", bufferSize);

            var actionBlock = new ActionBlock<TEntity[]>(OnReceive);
            Target.LinkTo(actionBlock);
            Target.Completion.ContinueWith(_ => actionBlock.Complete());
        }

        /// <summary>
        /// The block that connected to the inserter
        /// </summary>
        public BatchBlock<TEntity> Target { get; }

        private async Task OnReceive(IEnumerable<TEntity> submissions)
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
