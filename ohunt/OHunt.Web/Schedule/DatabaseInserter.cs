using System;
using System.Linq;
using System.Threading;
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
    /// An action block to buffer inputs and batch insert them to database
    /// </summary>
    public class DatabaseInserter<TEntity> : IDisposable, ITargetBlock<DatabaseInserterMessage<TEntity>>
        where TEntity : class
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<DatabaseInserter<TEntity>> _logger;
        private readonly TEntity[] _buffer;
        private readonly int _bufferSize;
        private readonly SemaphoreSlim _lock = new SemaphoreSlim(1, 1);
        private readonly ActionBlock<DatabaseInserterMessage<TEntity>> _target;

        private int _idx = 0;

        public DatabaseInserter(
            IServiceProvider serviceProvider,
            ILogger<DatabaseInserter<TEntity>> logger,
            IOptions<DatabaseInserterOptions> options)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;

            _bufferSize = options
                .Value.BufferSize.GetOrDefault(typeof(TEntity).Name, options.Value.DefaultBufferSize);
            _buffer = new TEntity[_bufferSize];

            _target
                = new ActionBlock<DatabaseInserterMessage<TEntity>>(OnReceive, new ExecutionDataflowBlockOptions
                {
                    EnsureOrdered = false,
                    MaxDegreeOfParallelism = 1,
                });
            Completion = _target.Completion.ContinueWith(async _ =>
            {
                await _lock.WaitAsync();
                await InsertAll();
                _lock.Release();
            });

            _logger.LogInformation("Initialized, buffer size: {0}", _bufferSize);
        }


        private async Task OnReceive(DatabaseInserterMessage<TEntity> message)
        {
            await _lock.WaitAsync();

            if (message.Entity != null)
            {
                Enqueue(message.Entity);
            }

            if (_idx >= _bufferSize || message.ForceInsert)
            {
                await InsertAll();
            }

            _lock.Release();
        }

        private void Enqueue(TEntity entity)
        {
            _buffer[_idx] = entity;
            ++_idx;
        }

        private async Task InsertAll()
        {
            _logger.LogTrace("Try to insert records to database");
            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<OHuntDbContext>();
            context.ChangeTracker.AutoDetectChangesEnabled = false;

            await context.Set<TEntity>().AddRangeAsync(_buffer.Take(_idx));
            _idx = 0;

            context.ChangeTracker.DetectChanges();
            var inserted = await context.SaveChangesAsync();

            _logger.LogInformation("{0} rows inserted", inserted);
        }

        public void Dispose()
        {
            _lock.Dispose();
        }

        public DataflowMessageStatus OfferMessage(
            DataflowMessageHeader messageHeader,
            DatabaseInserterMessage<TEntity> messageValue,
            ISourceBlock<DatabaseInserterMessage<TEntity>> source,
            bool consumeToAccept)
        {
            return ((ITargetBlock<DatabaseInserterMessage<TEntity>>) _target)
                .OfferMessage(messageHeader, messageValue, source, consumeToAccept);
        }

        public void Complete()
        {
            _target.Complete();
        }

        public void Fault(Exception exception)
        {
            ((ITargetBlock<DatabaseInserterMessage<TEntity>>) _target).Fault(exception);
        }

        public Task Completion { get; }
    }
}
