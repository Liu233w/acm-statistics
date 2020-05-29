using System.Collections.Generic;
using Abp.UI;
using AcmStatisticsBackend.Crawlers;
using AcmStatisticsBackend.ServiceClients;
using FluentAssertions;
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
                    CrawlerName = "cr1",
                    Solved = 1,
                    Submission = 3,
                    Username = "u1",
                    HasSolvedList = false,
                },
                new QueryWorkerHistory
                {
                    CrawlerName = "cr2",
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
            result.Solved.Should().Be(3);
            result.Submission.Should().Be(8);
            result.SummaryWarnings.Should().BeEquivalentTo(new List<SummaryWarning>
            {
                new SummaryWarning("cr1",
                    "This crawler does not have a solved list and " +
                    "its result will be directly added to summary."),
                new SummaryWarning("cr2",
                    "This crawler does not have a solved list and " +
                    "its result will be directly added to summary."),
            });

            result.QueryCrawlerSummaries.Should().BeEquivalentTo(new List<QueryCrawlerSummary>
            {
                new QueryCrawlerSummary
                {
                    CrawlerName = "cr1",
                    IsVirtualJudge = false,
                    Solved = 1,
                    Submission = 3,
                    Usernames = new List<UsernameInCrawler>
                    {
                        new UsernameInCrawler
                        {
                            Username = "u1",
                        },
                    },
                },
                new QueryCrawlerSummary
                {
                    CrawlerName = "cr2",
                    IsVirtualJudge = false,
                    Solved = 2,
                    Submission = 5,
                    Usernames = new List<UsernameInCrawler>
                    {
                        new UsernameInCrawler
                        {
                            Username = "u2",
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
                    CrawlerName = "cr3",
                    Solved = 3,
                    Submission = 10,
                    IsVirtualJudge = true,
                    HasSolvedList = false,
                },
            };

            FluentActions.Invoking(() =>
                    SummaryGenerator.Generate(_crawlerMeta, histories))
                .Should().Throw<UserFriendlyException>()
                .WithMessage("Virtual judge Cr3 should have a solved list.");
        }

        [Fact]
        public void WhenWorkerNotMatchCrawlerMeta_ShouldThrow()
        {
            var histories = new[]
            {
                new QueryWorkerHistory
                {
                    CrawlerName = "cr3",
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

            FluentActions.Invoking(() =>
                    SummaryGenerator.Generate(_crawlerMeta, histories))
                .Should().Throw<UserFriendlyException>()
                .WithMessage("According to crawler meta, " +
                             "the type of crawler Cr3 should be a virtual judge.");

            var histories2 = new[]
            {
                new QueryWorkerHistory
                {
                    CrawlerName = "cr1",
                    Solved = 1,
                    Submission = 10,
                    IsVirtualJudge = true,
                    HasSolvedList = true,
                    SolvedList = new[]
                    {
                        "cr2-1001",
                    },
                    SubmissionsByCrawlerName = new Dictionary<string, int>
                    {
                        { "cr2", 10 },
                    },
                },
            };

            FluentActions.Invoking(() =>
                    SummaryGenerator.Generate(_crawlerMeta, histories2))
                .Should().Throw<UserFriendlyException>()
                .WithMessage("According to crawler meta, " +
                             "the type of crawler Cr1 should not be a virtual judge.");
        }

        [Fact]
        public void WhenWorkerDoesNotExistInMeta_ShouldThrow()
        {
            var histories = new[]
            {
                new QueryWorkerHistory
                {
                    Submission = 1,
                    Solved = 1,
                    Username = "u5",
                    CrawlerName = "cr5",
                    IsVirtualJudge = false,
                    HasSolvedList = false,
                },
            };

            FluentActions.Invoking(() =>
                    SummaryGenerator.Generate(_crawlerMeta, histories))
                .Should().Throw<UserFriendlyException>()
                .WithMessage("Crawler cr5 does not exist in crawler meta.");
        }
    }
}
