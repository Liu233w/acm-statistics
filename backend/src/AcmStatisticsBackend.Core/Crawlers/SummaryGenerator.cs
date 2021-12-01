using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Linq;
using Abp.Dependency;
using Abp.Extensions;
using Abp.Timing;
using Abp.UI;
using AcmStatisticsBackend.ServiceClients;

namespace AcmStatisticsBackend.Crawlers
{
    /// <summary>
    /// Generate summarise from <see cref="QueryHistory"/>
    /// </summary>
    public class SummaryGenerator : ISingletonDependency
    {
        private readonly IClockProvider _clockProvider;

        public SummaryGenerator(IClockProvider clockProvider)
        {
            _clockProvider = clockProvider;
        }

        /// <summary>
        /// Generate summary from <see cref="QueryHistory"/>.
        /// <see cref="QueryHistory.QueryWorkerHistories"/> should already be loaded.
        ///
        /// It will not modify the parameter.
        /// </summary>
        [Pure]
        public QuerySummary Generate(
            IReadOnlyCollection<CrawlerMetaItem> crawlerMeta,
            IReadOnlyCollection<QueryWorkerHistory> workerHistories)
        {
            var histories = workerHistories
                .Where(item => item.ErrorMessage.IsNullOrEmpty())
                .ToList();

            ResolveSummaryData(crawlerMeta, histories,
                out var summaries,
                out var warnings,
                out var directlyAddSolvedWorkerList);

            foreach (var worker in directlyAddSolvedWorkerList)
            {
                var summary = summaries[worker.CrawlerName];
                summary.Usernames.Add(new UsernameInCrawler
                {
                    Username = worker.Username,
                });
            }

            var localJudgeDict = summaries
                .Where(it => it.Value.IsVirtualJudge == false)
                .ToDictionary(
                    p => p.Key,
                    p => new QueryCrawlerSummary
                    {
                        CrawlerName = p.Value.CrawlerName,
                        Solved = p.Value.SolvedSet.Count,
                        Submission = p.Value.Submissions,
                        Usernames = p.Value.Usernames.ToList(),
                        IsVirtualJudge = p.Value.IsVirtualJudge,
                    });

            // directlyAddSolvedWorkerList only exists in local judges
            foreach (var worker in directlyAddSolvedWorkerList)
            {
                var summary = localJudgeDict[worker.CrawlerName];
                summary.Solved += worker.Solved;
                summary.Submission += worker.Submission;
            }

            var virtualJudgeList = summaries
                .Select(it => it.Value)
                .Where(it => it.IsVirtualJudge)
                .SelectMany(it => new[]
                {
                    new QueryCrawlerSummary
                    {
                        CrawlerName = it.CrawlerName,
                        Solved = it.SolvedSet.Count,
                        Submission = it.Submissions,
                        Usernames = it.Usernames.ToList(),
                        IsVirtualJudge = false,
                    },
                    new QueryCrawlerSummary
                    {
                        CrawlerName = it.CrawlerName,
                        Solved = it.NotMergedSolvedSet.Count,
                        Submission = it.NotMergedSubmissions,
                        Usernames = it.NotMergedUsernames.ToList(),
                        IsVirtualJudge = true,
                    },
                });

            var summaryList = localJudgeDict
                .Select(p => p.Value)
                .Concat(virtualJudgeList)
                .Where(a => a.Usernames.Count > 0
                            && (a.Submission > 0 || a.Solved > 0))
                .OrderBy(a => a.CrawlerName)
                .ToList();

            return new QuerySummary
            {
                QueryCrawlerSummaries = summaryList,
                SummaryWarnings = warnings,
                Solved = summaryList.Sum(a => a.Solved),
                Submission = summaryList.Sum(a => a.Submission),
                GenerateTime = _clockProvider.Now,
            };
        }

        /// <summary>
        /// Pre-process data
        /// </summary>
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
                });
                if (summary.IsVirtualJudge)
                {
                    summary.NotMergedUsernames.Add(new UsernameInCrawler
                    {
                        Username = worker.Username,
                    });
                }

                if (worker.SolvedList == null)
                {
                    Debug.Assert(worker.IsVirtualJudge == false,
                        "All virtual judges should have solved list");
                    warnings.Add(new SummaryWarning(
                        worker.CrawlerName,
                        "This crawler does not have a solved list and " +
                        "its result will be directly added to summary."));
                    directlyAddSolvedWorkerList.Add(worker);
                    continue;
                }

                if (worker.IsVirtualJudge)
                {
                    if (worker.SubmissionsByCrawlerName.Values.Sum() != worker.Submission)
                    {
                        warnings.Add(new SummaryWarning(worker.CrawlerName,
                            "submissionByCrawler field of this crawler does not match its submission field, " +
                            "and only results in submissionByCrawler are used."));
                    }

                    HandleVirtualJudgeProblems(worker, summary, summaries);
                    HandleVirtualJudgeSubmissions(worker, summary, summaries);
                }
                else
                {
                    summary.Submissions += worker.Submission;
                    summary.SolvedSet.UnionWith(worker.SolvedList);
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
                    if (hasSolvedList != (history.SolvedList != null))
                    {
                        var title = GetCrawlerTitle(crawlerMeta, history);
                        throw new UserFriendlyException($"All workers of crawler {title} must have solved list!");
                    }
                }
                else
                {
                    workerHasSolvedList.Add(history.CrawlerName, history.SolvedList != null);
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

                if (history.IsVirtualJudge && history.SolvedList == null)
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

                if (summaries.TryGetValue(problemCrawlerName, out var problemCrawlerSummary))
                {
                    problemCrawlerSummary.Usernames.Add(new UsernameInCrawler
                    {
                        Username = worker.Username,
                        FromCrawlerName = worker.CrawlerName == problemCrawlerName
                            ? null
                            : worker.CrawlerName,
                    });
                    problemCrawlerSummary.SolvedSet.Add(problemId);
                }
                else
                {
                    vjSummary.NotMergedSolvedSet.Add(problem);
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

                    crawlerSummary.Usernames.Add(new UsernameInCrawler
                    {
                        Username = worker.Username,
                        FromCrawlerName = worker.CrawlerName == crawler
                            ? null
                            : worker.CrawlerName,
                    });
                }
                else
                {
                    vjSummary.NotMergedSubmissions += submissions;
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

            // in virtual judge, the two items means its local judge result
            public HashSet<string> SolvedSet { get; } = new HashSet<string>();
            public int Submissions { get; set; }
            public HashSet<UsernameInCrawler> Usernames { get; }

            // only work in virtual judge
            public HashSet<string> NotMergedSolvedSet { get; } = new HashSet<string>();
            public int NotMergedSubmissions { get; set; }
            public HashSet<UsernameInCrawler> NotMergedUsernames { get; }

            public CrawlerSummaryData()
            {
                Usernames = new HashSet<UsernameInCrawler>(new UsernameInCrawlerEqualityComparer());
                NotMergedUsernames = new HashSet<UsernameInCrawler>(new UsernameInCrawlerEqualityComparer());
            }
        }

        private class UsernameInCrawlerEqualityComparer : IEqualityComparer<UsernameInCrawler>
        {
            public bool Equals(UsernameInCrawler x, UsernameInCrawler y)
            {
                if (x == null && y == null)
                {
                    return true;
                }

                if (x == null || y == null)
                {
                    return false;
                }

                return x.Username == y.Username && x.FromCrawlerName == y.FromCrawlerName;
            }

            public int GetHashCode(UsernameInCrawler obj)
            {
                return $"{obj.FromCrawlerName ?? string.Empty}/{obj.Username ?? string.Empty}"
                    .GetHashCode();
            }
        }
    }
}
