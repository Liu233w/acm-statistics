using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using Abp.UI;
using AcmStatisticsBackend.ServiceClients;

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
        public static QuerySummary Generate(
            IReadOnlyCollection<CrawlerMetaItem> crawlerMeta,
            IReadOnlyCollection<QueryWorkerHistory> workerHistories)
        {
            ResolveSummaryData(crawlerMeta, workerHistories,
                out var summaries,
                out var warnings,
                out var directlyAddSolvedWorkerList);

            var summaryDict = summaries.ToDictionary(
                p => p.Key,
                p => new QueryCrawlerSummary
                {
                    CrawlerName = p.Value.CrawlerName,
                    Solved = p.Value.SolvedSet.Count,
                    Submission = p.Value.Submissions,
                    Usernames = p.Value.Usernames,
                    IsVirtualJudge = p.Value.IsVirtualJudge,
                });

            foreach (var worker in directlyAddSolvedWorkerList)
            {
                var summary = summaryDict[worker.CrawlerName];
                summary.Solved += worker.Solved;
            }

            var summaryList = summaryDict
                .Select(p => p.Value)
                .Where(a => a.Usernames.Count > 0)
                .ToList();

            return new QuerySummary
            {
                QueryCrawlerSummaries = summaryList,
                SummaryWarnings = warnings,
                Solved = summaryList.Sum(a => a.Solved),
                Submission = summaryList.Sum(a => a.Submission),
            };
        }

        private static void ResolveSummaryData(
            IReadOnlyCollection<CrawlerMetaItem> crawlerMeta,
            IReadOnlyCollection<QueryWorkerHistory> workerHistories,
            out Dictionary<string, CrawlerSummaryData> summaries,
            out List<SummaryWarning> warnings,
            out List<QueryWorkerHistory> directlyAddSolvedWorkerList)
        {
            summaries = InitSummaries(crawlerMeta);
            warnings = new List<SummaryWarning>();
            directlyAddSolvedWorkerList = new List<QueryWorkerHistory>();

            EnsureCrawlerType(crawlerMeta, summaries, workerHistories);

            foreach (var worker in workerHistories)
            {
                var summary = summaries[worker.CrawlerName];
                summary.Usernames.Add(new UsernameInCrawler
                {
                    Username = worker.Username,
                    FromCrawlerName = null,
                });
                summary.Submissions += worker.Submission;

                if (!worker.HasSolvedList)
                {
                    warnings.Add(new SummaryWarning(
                        worker.CrawlerName,
                        "This crawler does not have a solved list and " +
                        "its result will be directly added to summary."));
                    directlyAddSolvedWorkerList.Add(worker);
                    continue;
                }

                if (worker.IsVirtualJudge)
                {
                    HandleVirtualJudgeProblems(worker, summary, summaries);
                    HandleVirtualJudgeSubmissions(worker, summary, summaries);
                }
                else
                {
                    summary.SolvedSet = worker.SolvedList.ToHashSet();
                }
            }
        }

        private static void EnsureCrawlerType(
            IReadOnlyCollection<CrawlerMetaItem> crawlerMeta,
            IReadOnlyDictionary<string, CrawlerSummaryData> summaries,
            IReadOnlyCollection<QueryWorkerHistory> workerHistories)
        {
            var workerHasSolvedList = new Dictionary<string, bool>();

            foreach (var history in workerHistories)
            {
                if (workerHasSolvedList.TryGetValue(history.CrawlerName, out var hasSolvedList))
                {
                    if (hasSolvedList != history.HasSolvedList)
                    {
                        var title = GetCrawlerTitle(crawlerMeta, history);
                        throw new UserFriendlyException($"All workers of crawler {title} must have solved list!");
                    }
                }
                else
                {
                    workerHasSolvedList.Add(history.CrawlerName, history.HasSolvedList);
                }

                if (!summaries.TryGetValue(history.CrawlerName, out var summary))
                {
                    throw new UserFriendlyException(
                        $"The meta data of crawler {history.CrawlerName} does not exist.");
                }

                if (summary.IsVirtualJudge != history.IsVirtualJudge)
                {
                    var title = GetCrawlerTitle(crawlerMeta, history);
                    if (summary.IsVirtualJudge)
                    {
                        throw new UserFriendlyException(
                            $"According to crawler meta, the type of crawler {title} should be a virtual judge.");
                    }
                    else
                    {
                        throw new UserFriendlyException(
                            $"According to crawler meta, the type of crawler {title} should not be a virtual judge.");
                    }
                }

                if (history.IsVirtualJudge && !history.HasSolvedList)
                {
                    var title = GetCrawlerTitle(crawlerMeta, history);
                    throw new UserFriendlyException($"Virtual judge {title} should have a solved list.");
                }
            }
        }

        private static string GetCrawlerTitle(
            IReadOnlyCollection<CrawlerMetaItem> crawlerMeta,
            QueryWorkerHistory history)
        {
            var meta = crawlerMeta.First(item => item.CrawlerName == history.CrawlerName);
            return meta.CrawlerTitle;
        }

        private static void HandleVirtualJudgeProblems(
            QueryWorkerHistory worker,
            CrawlerSummaryData vjSummary,
            IReadOnlyDictionary<string, CrawlerSummaryData> summaries)
        {
            foreach (var problem in worker.SolvedList)
            {
                var (problemCrawlerName, problemId)
                    = problem.Split('-');

                if (summaries.TryGetValue(problemCrawlerName,
                    out var problemCrawlerSummary))
                {
                    problemCrawlerSummary.Usernames.Add(new UsernameInCrawler
                    {
                        Username = worker.Username,
                        FromCrawlerName = worker.CrawlerName,
                    });
                    problemCrawlerSummary.SolvedSet.Add(problemId);
                }
                else
                {
                    vjSummary.SolvedSet.Add(problem);
                }
            }
        }

        private static void HandleVirtualJudgeSubmissions(
            QueryWorkerHistory worker,
            CrawlerSummaryData vjSummary,
            IReadOnlyDictionary<string, CrawlerSummaryData> summaries)
        {
            foreach (var (crawler, submissions) in worker.SubmissionsByCrawlerName)
            {
                if (summaries.TryGetValue(crawler, out var crawlerSummary))
                {
                    crawlerSummary.Submissions += submissions;
                    vjSummary.Submissions -= submissions;
                }
            }
        }

        private static Dictionary<string, CrawlerSummaryData> InitSummaries(
            IReadOnlyCollection<CrawlerMetaItem> crawlerMeta)
        {
            return crawlerMeta
                .ToDictionary(
                    crawlerMetaItem => crawlerMetaItem.CrawlerName,
                    crawlerMetaItem => new CrawlerSummaryData
                    {
                        CrawlerName = crawlerMetaItem.CrawlerName,
                        IsVirtualJudge = crawlerMetaItem.IsVirtualJudge,
                    });
        }

        /// <summary>
        /// Data structure to use inside the algorithm
        /// </summary>
        private class CrawlerSummaryData
        {
            public string CrawlerName { get; set; }
            public bool IsVirtualJudge { get; set; }
            public HashSet<UsernameInCrawler> Usernames { get; set; } = new HashSet<UsernameInCrawler>();
            public HashSet<string> SolvedSet { get; set; } = new HashSet<string>();
            public int Submissions { get; set; } = 0;
        }
    }
}
