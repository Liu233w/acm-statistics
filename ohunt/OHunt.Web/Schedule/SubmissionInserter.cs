using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
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

        private static readonly List<PropertyInfo> Properties =
            (from property in typeof(Submission).GetProperties()
                select property).ToList();

        private readonly IServiceProvider _serviceProvider;

        public SubmissionInserter(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task Work(ISourceBlock<Submission> source)
        {
            while (await source.OutputAvailableAsync())
            {
                using var tempFile = new TempFile();
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
                        Properties.Select(p => p.GetValue(submission)?.ToString() ?? "")));
                }

                await Insert(tempFile);
            }
        }

        private async Task Insert(TempFile tempFile)
        {
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
            loader.Columns.AddRange(Properties.Select(p => p.Name));

            await loader.LoadAsync();
        }
    }
}
