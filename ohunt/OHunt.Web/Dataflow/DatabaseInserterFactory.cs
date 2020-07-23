using System;
using AngleSharp.Common;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OHunt.Web.Options;

namespace OHunt.Web.Dataflow
{
    public class DatabaseInserterFactory
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILoggerFactory _loggerFactory;
        private readonly IOptions<DatabaseInserterOptions> _options;

        public DatabaseInserterFactory(
            IServiceProvider serviceProvider,
            IOptions<DatabaseInserterOptions> options,
            ILoggerFactory loggerFactory)
        {
            _serviceProvider = serviceProvider;
            _options = options;
            _loggerFactory = loggerFactory;
        }

        public DatabaseInserter<TEntity> CreateInstance<TEntity>()
            where TEntity : class
        {
            var bufferSize = _options
                .Value.BufferSize.GetOrDefault(typeof(TEntity).Name, _options.Value.DefaultBufferSize);
            return new DatabaseInserter<TEntity>(
                _serviceProvider,
                _loggerFactory.CreateLogger($"DatabaseInserter({typeof(TEntity).Name})"),
                bufferSize);
        }
    }
}
