using System;
using System.IO;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MySql.Data.MySqlClient;
using OHunt.Web.Database;
using OHunt.Web.Models;
using OHunt.Web.Utils;

namespace OHunt.Web.Schedule
{
    /// <summary>
    /// Act as a consumer that reads Submission records and write them to the database.
    /// </summary>
    public class SubmissionInserter
    {
        private const int MaxRecordCount = 10000;

        private const string FieldSeparator = "\t";
        private const string LineSeparator = "\n";

        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<SubmissionInserter> _logger;

        public SubmissionInserter(
            IServiceProvider serviceProvider,
            ILogger<SubmissionInserter> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        public async Task WorkAsync(ISourceBlock<Submission> source)
        {
            while (await source.OutputAvailableAsync())
            {
                using var tempFile = new TempFile();
                _logger.LogTrace("Temp file created {0}", tempFile.Path);
                await using var writer = new StreamWriter(tempFile.Path)
                {
                    NewLine = LineSeparator,
                };
                for (var i = 0;
                    i < MaxRecordCount && await source.OutputAvailableAsync();
                    i++)
                {
                    var submission = await source.ReceiveAsync();
                    await writer.WriteLineAsync(string.Join(
                        FieldSeparator,
                        submission.GetValues()));
                }

                await Insert(tempFile);
            }
        }

        private async Task Insert(TempFile tempFile)
        {
            _logger.LogTrace("Try to save file {0} to database", tempFile.Path);
            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<OHuntWebContext>();

            var loader = new MySqlBulkLoader(
                context.Database.GetDbConnection() as MySqlConnection
                ?? throw new Exception("Current database is not mysql"))
            {
                Local = true,
                FileName = tempFile.Path,
                FieldTerminator = FieldSeparator,
                LineTerminator = LineSeparator,
                TableName = "submissions",
            };
            loader.Columns.AddRange(Submission.GetHeaders());

            var inserted = await loader.LoadAsync();
            _logger.LogTrace("{0} rows inserted", inserted);
        }
    }
}
