using System;
using System.Collections.Generic;
using System.Linq;
using Abp.UI;
using AcmStatisticsBackend.Crawlers;
using AcmStatisticsBackend.ServiceClients;
using Shouldly;
using Xunit;

namespace AcmStatisticsBackend.Tests.Crawlers
{
    public class SummaryGenerator_Tests : AcmStatisticsBackendTestBase
    {
        private readonly CrawlerMetaItem[] _crawlerMeta;

        public SummaryGenerator_Tests()
        {
            _crawlerMeta = new[]
            {
                new CrawlerMetaItem
                {
                    CrawlerName = "cr1",
                    CrawlerTitle = "Cr1",
                    Url = "",
                    CrawlerDescription = "",
                    IsVirtualJudge = false,
                },
                new CrawlerMetaItem
                {
                    CrawlerName = "cr2",
                    CrawlerTitle = "Cr2",
                    Url = "",
                    CrawlerDescription = "",
                    IsVirtualJudge = false,
                },
                new CrawlerMetaItem
                {
                    CrawlerName = "cr3",
                    CrawlerTitle = "Cr3",
                    Url = "",
                    CrawlerDescription = "",
                    IsVirtualJudge = true,
                },
                new CrawlerMetaItem
                {
                    CrawlerName = "cr4",
                    CrawlerTitle = "Cr4",
                    Url = "",
                    CrawlerDescription = "",
                    IsVirtualJudge = true,
                },
            };
        }

        [Fact]
        public void WithoutSolvedList_ShouldGenerateWarning()
        {
            // arrange
            var histories = new[]
            {
                new QueryWorkerHistory
                {
                    Solved = 1,
                    Submission = 3,
                    Username = "u1",
                    HasSolvedList = false,
                },
                new QueryWorkerHistory
                {
                    Solved = 2,
                    Submission = 5,
                    Username = "u2",
                    HasSolvedList = false,
                },
            };

            // act
            var result = SummaryGenerator.Generate(
                _crawlerMeta,
                histories);

            // assert
            result.Solved.ShouldBe(7);
            result.Submission.ShouldBe(18);
            result.SummaryWarnings.ShouldEqualInJson(new List<SummaryWarning>
            {
                new SummaryWarning("c1",
                    "This crawler does not have a solved list and " +
                    "its result will be directly added to summary."),
                new SummaryWarning("c2",
                    "This crawler does not have a solved list and " +
                    "its result will be directly added to summary."),
            });

            result.QueryCrawlerSummaries.ShouldEqualInJson(new List<QueryCrawlerSummary>
            {
                new QueryCrawlerSummary
                {
                    CrawlerName = "c1",
                    IsVirtualJudge = false,
                    Solved = 1,
                    Submission = 3,
                    Usernames = new List<UsernameInCrawler>
                    {
                        new UsernameInCrawler
                        {
                            Username = "u1",
                            FromCrawlerName = "",
                        },
                    },
                },
                new QueryCrawlerSummary
                {
                    CrawlerName = "c2",
                    IsVirtualJudge = false,
                    Solved = 2,
                    Submission = 5,
                    Usernames = new List<UsernameInCrawler>
                    {
                        new UsernameInCrawler
                        {
                            Username = "u2",
                            FromCrawlerName = "",
                        },
                    },
                },
            });
        }

        [Fact]
        public void VirtualJudgeWithoutSolvedList_ShouldThrow()
        {
            var histories = new[]
            {
                new QueryWorkerHistory
                {
                    CrawlerName = "c3",
                    Solved = 3,
                    Submission = 10,
                    IsVirtualJudge = true,
                    HasSolvedList = false,
                },
            };

            var call = new Func<QuerySummary>(() =>
                SummaryGenerator.Generate(_crawlerMeta, histories));
            call.ShouldThrow<UserFriendlyException>();
        }

        [Fact]
        public void WhenWorkerNotMatchCrawlerMeta_ShouldThrow()
        {
            var histories = new[]
            {
                new QueryWorkerHistory
                {
                    CrawlerName = "c3",
                    Solved = 1,
                    Submission = 10,
                    IsVirtualJudge = false,
                    HasSolvedList = true,
                    SolvedList = new[]
                    {
                        "moj-1001",
                    },
                },
            };

            var call = new Func<QuerySummary>(() =>
                SummaryGenerator.Generate(_crawlerMeta, histories));
            call.ShouldThrow<UserFriendlyException>();
        }
    }
}
