using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using Abp.Dependency;
using Abp.UI;

namespace AcmStatisticsBackend.Crawlers
{
    /// <summary>
    /// Generate summarise from <see cref="QueryHistory"/>
    /// </summary>
    public static class SummaryGenerator
    {
        /// <summary>
        /// Generate summary from <see cref="QueryHistory"/>.
        /// <see cref="QueryHistory.QueryWorkerHistories"/> should already be loaded.
        ///
        /// It will not modify the parameter.
        /// </summary>
        [Pure]
        public static QuerySummary Generate(QueryHistory history)
        {
            var warnings = new List<SummaryWarning>();
            var nameToCrawlerSummary = new Dictionary<string, CrawlerSummaryData>();
            var directlyAddWorker = new List<QueryWorkerHistory>();

            // init virtual judges and other summary
            foreach (var workerHistory in history.QueryWorkerHistories)
            {
                if (nameToCrawlerSummary.TryGetValue(workerHistory.CrawlerName, out var summary))
                {
                    EnsureVirtualJudgeIdentical(summary, workerHistory);
                    continue;
                }

                var newSummary = new CrawlerSummaryData
                {
                    CrawlerName = workerHistory.CrawlerName,
                    IsVirtualJudge = workerHistory.IsVirtualJudge,
                };
                nameToCrawlerSummary.Add(workerHistory.CrawlerName, newSummary);
            }

            foreach (var workerHistory in history.QueryWorkerHistories)
            {
                if (!workerHistory.HasSolvedList)
                {
                    warnings.Add(new SummaryWarning(
                        workerHistory.CrawlerName,
                        "This crawler does not have a solved list and " +
                        "its result will be directly added to summary."));
                    directlyAddWorker.Add(workerHistory);
                    continue;
                }

                if (workerHistory.IsVirtualJudge)
                {
                    var vjSummary = nameToCrawlerSummary[workerHistory.CrawlerName];
                    foreach (var problem in workerHistory.SolvedList)
                    {
                        problem.Split('-').Deconstruct(
                            out var problemCrawlerName,
                            out var problemId);
                    }
                }

                var summary = GetOrAddSummaryData(nameToCrawlerSummary, workerHistory.CrawlerName);
                summary.Usernames.Add(new UsernameInCrawler())
            }

            throw new NotImplementedException();
        }

        private static CrawlerSummaryData GetOrAddSummaryData(
            IDictionary<string, CrawlerSummaryData> nameToCrawlerSummary,
            string crawlerName)
        {
            if (nameToCrawlerSummary.TryGetValue(crawlerName, out var summary))
            {
                return summary;
            }

            var newSummary = new CrawlerSummaryData
            {
                CrawlerName = crawlerName,
                IsVirtualJudge = false,
            };
            nameToCrawlerSummary.Add(crawlerName, newSummary);
            return newSummary;
        }

        private static void EnsureVirtualJudgeIdentical(
            CrawlerSummaryData crawlerSummaryData,
            QueryWorkerHistory workerHistory)
        {
            if (crawlerSummaryData.IsVirtualJudge != workerHistory.IsVirtualJudge)
            {
                throw new UserFriendlyException(
                    $"The type of crawler {crawlerSummaryData.CrawlerName} is not identical!",
                    "Their IsVirtualJudge field should be identical.");
            }
        }

        private class CrawlerSummaryData
        {
            public string CrawlerName { get; set; }
            public bool IsVirtualJudge { get; set; }
            public HashSet<UsernameInCrawler> Usernames { get; set; } = new HashSet<UsernameInCrawler>();
            public HashSet<string> Problems { get; set; } = new HashSet<string>();
        }
    }
}
