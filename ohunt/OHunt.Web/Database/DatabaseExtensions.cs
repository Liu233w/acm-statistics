using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MySql.Data.MySqlClient;
using OHunt.Web.Models;
using OHunt.Web.Utils;

namespace OHunt.Web.Database
{
    public static class DatabaseExtensions
    {
        private const string FieldSeparator = "\t";
        private const string LineSeparator = "\n";

        private static readonly List<PropertyInfo> Properties =
            (from property in typeof(Submission).GetProperties()
                select property).ToList();

        public static async Task InsertSubmissionsAsync(
            this OHuntWebContext context,
            IEnumerable<Submission> submissions)
        {
            using var tempFile = new TempFile();

            await using (var writer = new StreamWriter(tempFile.Path))
            {
                writer.NewLine = LineSeparator;
                foreach (var submission in submissions)
                {
                    await writer.WriteLineAsync(string.Join(
                        FieldSeparator,
                        Properties.Select(p => p.GetValue(submission)?.ToString() ?? "")));
                }
            }

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
