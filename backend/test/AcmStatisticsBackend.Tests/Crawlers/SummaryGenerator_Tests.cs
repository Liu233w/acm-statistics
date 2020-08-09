using System;
using System.Collections.Generic;
using Abp.UI;
using AcmStatisticsBackend.Crawlers;
using AcmStatisticsBackend.ServiceClients;
using FluentAssertions;
using Xunit;

namespace AcmStatisticsBackend.Tests.Crawlers
{
    public class SummaryGenerator_Tests
    {
        private readonly CrawlerMetaItem[] _crawlerMeta;
        private readonly SummaryGenerator _summaryGenerator;

        public SummaryGenerator_Tests()
        {
            var testClockProvider = new TestClockProvider();
            testClockProvider.Now = new DateTime(2020, 4, 1);

            _summaryGenerator = new SummaryGenerator(testClockProvider);

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
        public void It_ShouldHaveGenerateTime()
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
                },
            };

            // act
            var result = _summaryGenerator.Generate(
                _crawlerMeta,
                histories);

            // assert
            result.GenerateTime.Should().Be(new DateTime(2020, 4, 1));
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
                },
                new QueryWorkerHistory
                {
                    CrawlerName = "cr2",
                    Solved = 2,
                    Submission = 5,
                    Username = "u2",
                },
            };

            // act
            var result = _summaryGenerator.Generate(
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
                },
            };

            FluentActions.Invoking(() =>
                    _summaryGenerator.Generate(_crawlerMeta, histories))
                .Should().Throw<UserFriendlyException>()
                .WithMessage("Virtual judge Cr3 should have a solved list.");
        }

        [Theory]
        [InlineData("cr3", false, "According to crawler meta, the type of crawler Cr3 should be a virtual judge.")]
        [InlineData("cr1", true, "According to crawler meta, the type of crawler Cr1 should not be a virtual judge.")]
        public void WhenWorkerNotMatchCrawlerMeta_ShouldThrow(
            string crawlerName,
            bool isVirtualJudge,
            string exceptionMessage)
        {
            var histories = new[]
            {
                new QueryWorkerHistory
                {
                    CrawlerName = crawlerName,
                    Solved = 1,
                    Submission = 10,
                    IsVirtualJudge = isVirtualJudge,
                    SolvedList = new[]
                    {
                        "moj-1001",
                    },
                },
            };

            FluentActions.Invoking(() =>
                    _summaryGenerator.Generate(_crawlerMeta, histories))
                .Should().Throw<UserFriendlyException>()
                .WithMessage(exceptionMessage);
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
                },
            };

            FluentActions.Invoking(() =>
                    _summaryGenerator.Generate(_crawlerMeta, histories))
                .Should().Throw<UserFriendlyException>()
                .WithMessage("The meta data of crawler cr5 does not exist.");
        }

        [Fact]
        public void LocalJudgeWithSolvedList_ShouldWork()
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
                    SolvedList = new[]
                    {
                        "1001",
                    },
                },
                new QueryWorkerHistory
                {
                    CrawlerName = "cr2",
                    Solved = 2,
                    Submission = 5,
                    Username = "u2",
                    SolvedList = new[]
                    {
                        "1001",
                        "1002",
                    },
                },
            };

            // act
            var result = _summaryGenerator.Generate(
                _crawlerMeta,
                histories);

            // assert
            result.Solved.Should().Be(3);
            result.Submission.Should().Be(8);
            result.SummaryWarnings.Should().HaveCount(0);

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

        [Theory]
        [InlineData(
            new[] { "1001", },
            new[] { "1001", "1002", },
            2)]
        [InlineData(
            new[] { "1001", "1005", },
            new[] { "1001", "1002", },
            3)]
        [InlineData(
            new[] { "1003", "1005", },
            new[] { "1001", "1002", },
            4)]
        [InlineData(
            new[] { "1001", "1002", },
            new[] { "1001", "1002", },
            2)]
        public void DifferentWorkerOfTheSameCrawler_ShouldMergeTheirSolvedList(string[] set1, string[] set2, int solved)
        {
            // arrange
            var histories = new[]
            {
                new QueryWorkerHistory
                {
                    CrawlerName = "cr1",
                    Solved = set1.Length,
                    Submission = 20,
                    Username = "u1",
                    SolvedList = set1,
                },
                new QueryWorkerHistory
                {
                    CrawlerName = "cr1",
                    Solved = set2.Length,
                    Submission = 30,
                    Username = "u2",
                    SolvedList = set2,
                },
            };

            // act
            var result = _summaryGenerator.Generate(
                _crawlerMeta,
                histories);

            // assert
            result.Solved.Should().Be(solved);
            result.Submission.Should().Be(50);
            result.SummaryWarnings.Should().HaveCount(0);

            result.QueryCrawlerSummaries.Should().BeEquivalentTo(new List<QueryCrawlerSummary>
            {
                new QueryCrawlerSummary
                {
                    CrawlerName = "cr1",
                    IsVirtualJudge = false,
                    Solved = solved,
                    Submission = 50,
                    Usernames = new List<UsernameInCrawler>
                    {
                        new UsernameInCrawler
                        {
                            Username = "u1",
                        },
                        new UsernameInCrawler
                        {
                            Username = "u2",
                        },
                    },
                },
            });
        }

        [Fact]
        public void DifferentWorkerOfTheSameCrawler_WhenOnlySomeWorkerDoNotHaveSolvedList_ShouldThrow()
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
                    SolvedList = new[]
                    {
                        "1001",
                    },
                },
                new QueryWorkerHistory
                {
                    CrawlerName = "cr1",
                    Solved = 2,
                    Submission = 5,
                    Username = "u2",
                    SolvedList = new[]
                    {
                        "1001",
                        "1002",
                    },
                },
                new QueryWorkerHistory
                {
                    CrawlerName = "cr1",
                    Solved = 10,
                    Submission = 10,
                    Username = "u3",
                },
            };

            // act
            var call = FluentActions.Invoking(() => _summaryGenerator.Generate(
                _crawlerMeta,
                histories));

            // assert
            call.Should().Throw<UserFriendlyException>()
                .WithMessage("All workers of crawler Cr1 must have solved list!");
        }

        [Fact]
        public void WorkerCrawlerExistsInVirtualJudge_WhenBothHaveSolvedList_ShouldMergeResult()
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
                    SolvedList = new[]
                    {
                        "1001",
                    },
                },
                new QueryWorkerHistory
                {
                    CrawlerName = "cr3",
                    IsVirtualJudge = true,
                    Solved = 3,
                    Submission = 15,
                    Username = "u2",
                    SolvedList = new[]
                    {
                        "cr1-1001",
                        "cr1-1002",
                        "cr2-2001",
                    },
                    SubmissionsByCrawlerName = new Dictionary<string, int>
                    {
                        { "cr1", 5 },
                        { "cr2", 10 },
                    },
                },
            };

            // act
            var result = _summaryGenerator.Generate(
                _crawlerMeta,
                histories);

            // assert
            result.Solved.Should().Be(3);
            result.Submission.Should().Be(18);
            result.SummaryWarnings.Should().HaveCount(0);

            result.QueryCrawlerSummaries.Should().BeEquivalentTo(new List<QueryCrawlerSummary>
            {
                new QueryCrawlerSummary
                {
                    CrawlerName = "cr1",
                    IsVirtualJudge = false,
                    Solved = 2,
                    Submission = 8,
                    Usernames = new List<UsernameInCrawler>
                    {
                        new UsernameInCrawler
                        {
                            Username = "u1",
                        },
                        new UsernameInCrawler
                        {
                            FromCrawlerName = "cr3",
                            Username = "u2",
                        },
                    },
                },
                new QueryCrawlerSummary
                {
                    CrawlerName = "cr2",
                    IsVirtualJudge = false,
                    Solved = 1,
                    Submission = 10,
                    Usernames = new List<UsernameInCrawler>
                    {
                        new UsernameInCrawler
                        {
                            FromCrawlerName = "cr3",
                            Username = "u2",
                        },
                    },
                },
            });
        }

        [Fact]
        public void WorkerCrawlerExistsInVirtualJudge_WhenCrawlerNotHaveSolvedList_ShouldGenerateWarning_AndAddResult()
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
                },
                new QueryWorkerHistory
                {
                    CrawlerName = "cr3",
                    IsVirtualJudge = true,
                    Solved = 3,
                    Submission = 15,
                    Username = "u2",
                    SolvedList = new[]
                    {
                        "cr1-1001",
                        "cr1-1002",
                        "cr2-2001",
                    },
                    SubmissionsByCrawlerName = new Dictionary<string, int>
                    {
                        { "cr1", 5 },
                        { "cr2", 10 },
                    },
                },
            };

            // act
            var result = _summaryGenerator.Generate(
                _crawlerMeta,
                histories);

            // assert
            result.Solved.Should().Be(4);
            result.Submission.Should().Be(18);
            result.SummaryWarnings.Should().BeEquivalentTo(new List<SummaryWarning>
            {
                new SummaryWarning("cr1",
                    "This crawler does not have a solved list and " +
                    "its result will be directly added to summary."),
            });

            result.QueryCrawlerSummaries.Should().BeEquivalentTo(new List<QueryCrawlerSummary>
            {
                new QueryCrawlerSummary
                {
                    CrawlerName = "cr1",
                    IsVirtualJudge = false,
                    Solved = 3,
                    Submission = 8,
                    Usernames = new List<UsernameInCrawler>
                    {
                        new UsernameInCrawler
                        {
                            Username = "u1",
                        },
                        new UsernameInCrawler
                        {
                            FromCrawlerName = "cr3",
                            Username = "u2",
                        },
                    },
                },
                new QueryCrawlerSummary
                {
                    CrawlerName = "cr2",
                    IsVirtualJudge = false,
                    Solved = 1,
                    Submission = 10,
                    Usernames = new List<UsernameInCrawler>
                    {
                        new UsernameInCrawler
                        {
                            FromCrawlerName = "cr3",
                            Username = "u2",
                        },
                    },
                },
            });
        }

        [Fact]
        public void WhenSolvedNotMatchSolvedList_ShouldUseResultOfSolvedList()
        {
            // arrange
            var histories = new[]
            {
                new QueryWorkerHistory
                {
                    CrawlerName = "cr1",
                    Solved = 2,
                    Submission = 3,
                    Username = "u1",
                    SolvedList = new[]
                    {
                        "1001",
                    },
                },
            };

            // act
            var result = _summaryGenerator.Generate(
                _crawlerMeta,
                histories);

            // assert
            result.Solved.Should().Be(1);
            result.Submission.Should().Be(3);
            // result.SummaryWarnings.Should().BeEquivalentTo(new List<SummaryWarning>
            // {
            //     new SummaryWarning("cr1",
            //         "The solved number of this crawler is 2, however, there are 1" +
            //         " problems in the solved list, which can be an error of the crawler."),
            // });
            result.SummaryWarnings.Should().HaveCount(0);

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
            });
        }

        [Fact]
        public void VirtualJudge_WhenSolvedNotMatchSolvedList_ShouldUseResultOfSolvedList()
        {
            // arrange
            var histories = new[]
            {
                new QueryWorkerHistory
                {
                    CrawlerName = "cr3",
                    IsVirtualJudge = true,
                    Solved = 2,
                    Submission = 5,
                    Username = "u1",
                    SolvedList = new[]
                    {
                        "cr1-1001",
                    },
                    SubmissionsByCrawlerName = new Dictionary<string, int>
                    {
                        { "cr1", 1 },
                        { "cr2", 4 },
                    },
                },
            };

            // act
            var result = _summaryGenerator.Generate(
                _crawlerMeta,
                histories);

            // assert
            result.Solved.Should().Be(1);
            result.Submission.Should().Be(5);
            // result.SummaryWarnings.Should().BeEquivalentTo(new List<SummaryWarning>
            // {
            //     new SummaryWarning("cr3",
            //         "The solved number of this crawler is 2, however, there are 1" +
            //         " problems in the solved list. Only "),
            // });
            result.SummaryWarnings.Should().HaveCount(0);

            result.QueryCrawlerSummaries.Should().BeEquivalentTo(new List<QueryCrawlerSummary>
            {
                new QueryCrawlerSummary
                {
                    CrawlerName = "cr1",
                    IsVirtualJudge = false,
                    Solved = 1,
                    Submission = 1,
                    Usernames = new List<UsernameInCrawler>
                    {
                        new UsernameInCrawler
                        {
                            FromCrawlerName = "cr3",
                            Username = "u1",
                        },
                    },
                },
                new QueryCrawlerSummary
                {
                    CrawlerName = "cr2",
                    IsVirtualJudge = false,
                    Solved = 0,
                    Submission = 4,
                    Usernames = new List<UsernameInCrawler>
                    {
                        new UsernameInCrawler
                        {
                            FromCrawlerName = "cr3",
                            Username = "u1",
                        },
                    },
                },
            });
        }

        [Fact]
        public void VirtualJudge_WhenAllSolvedListsMerged_ShouldNotExistInSummary()
        {
            // arrange
            var histories = new[]
            {
                new QueryWorkerHistory
                {
                    CrawlerName = "cr3",
                    IsVirtualJudge = true,
                    Solved = 1,
                    Submission = 15,
                    Username = "u1",
                    SolvedList = new[]
                    {
                        "cr1-1001",
                    },
                    SubmissionsByCrawlerName = new Dictionary<string, int>
                    {
                        { "cr1", 5 },
                        { "cr2", 10 },
                    },
                },
            };

            // act
            var result = _summaryGenerator.Generate(
                _crawlerMeta,
                histories);

            // assert
            result.Solved.Should().Be(1);
            result.Submission.Should().Be(15);
            result.SummaryWarnings.Should().HaveCount(0);

            result.QueryCrawlerSummaries.Should().BeEquivalentTo(new List<QueryCrawlerSummary>
            {
                new QueryCrawlerSummary
                {
                    CrawlerName = "cr1",
                    IsVirtualJudge = false,
                    Solved = 1,
                    Submission = 5,
                    Usernames = new List<UsernameInCrawler>
                    {
                        new UsernameInCrawler
                        {
                            FromCrawlerName = "cr3",
                            Username = "u1",
                        },
                    },
                },
                new QueryCrawlerSummary
                {
                    CrawlerName = "cr2",
                    IsVirtualJudge = false,
                    Solved = 0,
                    Submission = 10,
                    Usernames = new List<UsernameInCrawler>
                    {
                        new UsernameInCrawler
                        {
                            FromCrawlerName = "cr3",
                            Username = "u1",
                        },
                    },
                },
            });
        }

        [Fact]
        public void VirtualJudge_WhenSomeSolvedListsNotMerged_ShouldExistInSummary()
        {
            // arrange
            var histories = new[]
            {
                new QueryWorkerHistory
                {
                    CrawlerName = "cr3",
                    IsVirtualJudge = true,
                    Solved = 1,
                    Submission = 15,
                    Username = "u1",
                    SolvedList = new[]
                    {
                        "NN-1001",
                    },
                    SubmissionsByCrawlerName = new Dictionary<string, int>
                    {
                        { "cr1", 5 },
                        { "NN", 10 },
                    },
                },
            };

            // act
            var result = _summaryGenerator.Generate(
                _crawlerMeta,
                histories);

            // assert
            result.Solved.Should().Be(1);
            result.Submission.Should().Be(15);
            result.SummaryWarnings.Should().HaveCount(0);

            result.QueryCrawlerSummaries.Should().BeEquivalentTo(new List<QueryCrawlerSummary>
            {
                new QueryCrawlerSummary
                {
                    CrawlerName = "cr1",
                    IsVirtualJudge = false,
                    Solved = 0,
                    Submission = 5,
                    Usernames = new List<UsernameInCrawler>
                    {
                        new UsernameInCrawler
                        {
                            FromCrawlerName = "cr3",
                            Username = "u1",
                        },
                    },
                },
                new QueryCrawlerSummary
                {
                    CrawlerName = "cr3",
                    IsVirtualJudge = true,
                    Solved = 1,
                    Submission = 10,
                    Usernames = new List<UsernameInCrawler>
                    {
                        new UsernameInCrawler
                        {
                            Username = "u1",
                        },
                    },
                },
            });
        }

        [Fact]
        public void VirtualJudge_WhenSubmissionByCrawlerIsNotInCrawlerMeta_ShouldAddToVirtualJudge()
        {
            // arrange
            var histories = new[]
            {
                new QueryWorkerHistory
                {
                    CrawlerName = "cr3",
                    IsVirtualJudge = true,
                    Solved = 1,
                    Submission = 10,
                    Username = "u1",
                    SolvedList = new[]
                    {
                        "NN-1001",
                    },
                    SubmissionsByCrawlerName = new Dictionary<string, int>
                    {
                        { "NN", 10 },
                    },
                },
            };

            // act
            var result = _summaryGenerator.Generate(
                _crawlerMeta,
                histories);

            // assert
            result.Solved.Should().Be(1);
            result.Submission.Should().Be(10);
            result.SummaryWarnings.Should().HaveCount(0);

            result.QueryCrawlerSummaries.Should().BeEquivalentTo(new List<QueryCrawlerSummary>
            {
                new QueryCrawlerSummary
                {
                    CrawlerName = "cr3",
                    IsVirtualJudge = true,
                    Solved = 1,
                    Submission = 10,
                    Usernames = new List<UsernameInCrawler>
                    {
                        new UsernameInCrawler
                        {
                            Username = "u1",
                        },
                    },
                },
            });
        }

        [Fact]
        public void
            VirtualJudge_WhenSubmissionNotMatchSubmissionByCrawler_ShouldGenerateWarning_AndUseSubmissionByCrawler()
        {
            // arrange
            var histories = new[]
            {
                new QueryWorkerHistory
                {
                    CrawlerName = "cr3",
                    IsVirtualJudge = true,
                    Solved = 0,
                    Submission = 1,
                    Username = "u1",
                    SolvedList = new string[]
                    {
                    },
                    SubmissionsByCrawlerName = new Dictionary<string, int>
                    {
                        { "NN", 10 },
                    },
                },
            };

            // act
            var result = _summaryGenerator.Generate(
                _crawlerMeta,
                histories);

            // assert
            result.Solved.Should().Be(0);
            result.Submission.Should().Be(10);
            result.SummaryWarnings.Should().BeEquivalentTo(new List<SummaryWarning>
            {
                new SummaryWarning("cr3",
                    "submissionByCrawler field of this crawler does not match its submission field, " +
                    "and only results in submissionByCrawler are used."),
            });

            result.QueryCrawlerSummaries.Should().BeEquivalentTo(new List<QueryCrawlerSummary>
            {
                new QueryCrawlerSummary
                {
                    CrawlerName = "cr3",
                    IsVirtualJudge = true,
                    Solved = 0,
                    Submission = 10,
                    Usernames = new List<UsernameInCrawler>
                    {
                        new UsernameInCrawler
                        {
                            Username = "u1",
                        },
                    },
                },
            });
        }

        [Fact]
        public void VirtualJudge_WhenItHasLocalJudge_ShouldWorkAsLocalJudge()
        {
            // arrange
            var histories = new[]
            {
                new QueryWorkerHistory
                {
                    CrawlerName = "cr3",
                    IsVirtualJudge = true,
                    Solved = 1,
                    Submission = 20,
                    Username = "u1",
                    SolvedList = new[]
                    {
                        "cr3-1001",
                    },
                    SubmissionsByCrawlerName = new Dictionary<string, int>
                    {
                        { "cr3", 20 },
                    },
                },
            };

            // act
            var result = _summaryGenerator.Generate(
                _crawlerMeta,
                histories);

            // assert
            result.Solved.Should().Be(1);
            result.Submission.Should().Be(20);
            result.SummaryWarnings.Should().HaveCount(0);

            result.QueryCrawlerSummaries.Should().BeEquivalentTo(new List<QueryCrawlerSummary>
            {
                new QueryCrawlerSummary
                {
                    CrawlerName = "cr3",
                    IsVirtualJudge = false,
                    Solved = 1,
                    Submission = 20,
                    Usernames = new List<UsernameInCrawler>
                    {
                        new UsernameInCrawler
                        {
                            Username = "u1",
                        },
                    },
                },
            });
        }

        [Fact]
        public void VirtualJudge_WhenItHasLocalJudge_AndProvidedByAnotherVj_ShouldWorkAsLocalJudge()
        {
            // arrange
            var histories = new[]
            {
                new QueryWorkerHistory
                {
                    CrawlerName = "cr3",
                    IsVirtualJudge = true,
                    Solved = 1,
                    Submission = 20,
                    Username = "u1",
                    SolvedList = new[]
                    {
                        "cr3-1001",
                    },
                    SubmissionsByCrawlerName = new Dictionary<string, int>
                    {
                        { "cr3", 20 },
                    },
                },
                new QueryWorkerHistory
                {
                    CrawlerName = "cr4",
                    IsVirtualJudge = true,
                    Solved = 2,
                    Submission = 20,
                    Username = "u2",
                    SolvedList = new[]
                    {
                        "cr3-1001",
                        "cr3-1002",
                    },
                    SubmissionsByCrawlerName = new Dictionary<string, int>
                    {
                        { "cr3", 20 },
                    },
                },
            };

            // act
            var result = _summaryGenerator.Generate(
                _crawlerMeta,
                histories);

            // assert
            result.Solved.Should().Be(2);
            result.Submission.Should().Be(40);
            result.SummaryWarnings.Should().HaveCount(0);

            result.QueryCrawlerSummaries.Should().BeEquivalentTo(new List<QueryCrawlerSummary>
            {
                new QueryCrawlerSummary
                {
                    CrawlerName = "cr3",
                    IsVirtualJudge = false,
                    Solved = 2,
                    Submission = 40,
                    Usernames = new List<UsernameInCrawler>
                    {
                        new UsernameInCrawler
                        {
                            Username = "u1",
                        },
                        new UsernameInCrawler
                        {
                            Username = "u2",
                            FromCrawlerName = "cr4",
                        },
                    },
                },
            });
        }

        [Fact]
        public void VirtualJudge_WhenItHasLocalJudge_AndListNotMerged_ShouldOutputTwoSummary()
        {
            // arrange
            var histories = new[]
            {
                new QueryWorkerHistory
                {
                    CrawlerName = "cr3",
                    IsVirtualJudge = true,
                    Solved = 1,
                    Submission = 35,
                    Username = "u1",
                    SolvedList = new[]
                    {
                        "NN-1001",
                        "cr1-1001",
                        "cr3-1001",
                    },
                    SubmissionsByCrawlerName = new Dictionary<string, int>
                    {
                        { "cr1", 5 },
                        { "NN", 10 },
                        { "cr3", 20 },
                    },
                },
            };

            // act
            var result = _summaryGenerator.Generate(
                _crawlerMeta,
                histories);

            // assert
            result.Solved.Should().Be(3);
            result.Submission.Should().Be(35);
            result.SummaryWarnings.Should().HaveCount(0);

            result.QueryCrawlerSummaries.Should().BeEquivalentTo(new List<QueryCrawlerSummary>
            {
                new QueryCrawlerSummary
                {
                    CrawlerName = "cr1",
                    IsVirtualJudge = false,
                    Solved = 1,
                    Submission = 5,
                    Usernames = new List<UsernameInCrawler>
                    {
                        new UsernameInCrawler
                        {
                            FromCrawlerName = "cr3",
                            Username = "u1",
                        },
                    },
                },
                // cr3 should has 2 crawler summary
                // as local judge and virtual judge, separately
                new QueryCrawlerSummary
                {
                    CrawlerName = "cr3",
                    IsVirtualJudge = true,
                    Solved = 1,
                    Submission = 10,
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
                    CrawlerName = "cr3",
                    IsVirtualJudge = false,
                    Solved = 1,
                    Submission = 20,
                    Usernames = new List<UsernameInCrawler>
                    {
                        new UsernameInCrawler
                        {
                            Username = "u1",
                        },
                    },
                },
            });
        }

        [Fact]
        public void VirtualJudge_WhenItHasLocalJudge_CanOutputTwoDifferentUsernames()
        {
            // arrange
            var histories = new[]
            {
                new QueryWorkerHistory
                {
                    CrawlerName = "cr3",
                    IsVirtualJudge = true,
                    Solved = 1,
                    Submission = 35,
                    Username = "u1",
                    SolvedList = new[]
                    {
                        "NN-1001",
                        "cr1-1001",
                        "cr3-1001",
                    },
                    SubmissionsByCrawlerName = new Dictionary<string, int>
                    {
                        { "cr1", 5 },
                        { "NN", 10 },
                        { "cr3", 20 },
                    },
                },
                new QueryWorkerHistory
                {
                    CrawlerName = "cr4",
                    IsVirtualJudge = true,
                    Solved = 2,
                    Submission = 20,
                    Username = "u2",
                    SolvedList = new[]
                    {
                        "cr3-1001",
                        "cr3-1002",
                    },
                    SubmissionsByCrawlerName = new Dictionary<string, int>
                    {
                        { "cr3", 20 },
                    },
                },
            };

            // act
            var result = _summaryGenerator.Generate(
                _crawlerMeta,
                histories);

            // assert
            result.Solved.Should().Be(4);
            result.Submission.Should().Be(55);
            result.SummaryWarnings.Should().HaveCount(0);

            result.QueryCrawlerSummaries.Should().BeEquivalentTo(new List<QueryCrawlerSummary>
            {
                new QueryCrawlerSummary
                {
                    CrawlerName = "cr1",
                    IsVirtualJudge = false,
                    Solved = 1,
                    Submission = 5,
                    Usernames = new List<UsernameInCrawler>
                    {
                        new UsernameInCrawler
                        {
                            FromCrawlerName = "cr3",
                            Username = "u1",
                        },
                    },
                },
                // cr3 should has 2 crawler summary
                // as local judge and virtual judge, separately
                new QueryCrawlerSummary
                {
                    CrawlerName = "cr3",
                    IsVirtualJudge = true,
                    Solved = 1,
                    Submission = 10,
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
                    CrawlerName = "cr3",
                    IsVirtualJudge = false,
                    Solved = 2,
                    Submission = 40,
                    Usernames = new List<UsernameInCrawler>
                    {
                        new UsernameInCrawler
                        {
                            Username = "u1",
                        },
                        new UsernameInCrawler
                        {
                            Username = "u2",
                            FromCrawlerName = "cr4",
                        },
                    },
                },
            });
        }

        [Fact]
        public void WhenInputErrorWorker_ShouldIgnore()
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
                    SolvedList = new[]
                    {
                        "1001",
                    },
                },
                new QueryWorkerHistory
                {
                    CrawlerName = "cr3",
                    IsVirtualJudge = true,
                    Username = "u2",
                    ErrorMessage = "An error occured",
                },
            };

            // act
            var result = _summaryGenerator.Generate(
                _crawlerMeta,
                histories);

            // assert
            // assert
            result.Solved.Should().Be(1);
            result.Submission.Should().Be(3);
            result.SummaryWarnings.Should().BeEmpty();

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
            });
        }
    }
}
