using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using AcmStatisticsBackend.Crawlers;
using FluentAssertions;
using Xunit;

namespace AcmStatisticsBackend.Tests.Crawlers
{
    public class QuerySummary_ModelTests : AcmStatisticsBackendTestBase
    {
        private readonly IRepository<QueryHistory, long> _queryHistoryRepository;

        private readonly IRepository<QuerySummary, long> _querySummaryRepository;

        public QuerySummary_ModelTests()
        {
            _queryHistoryRepository = Resolve<IRepository<QueryHistory, long>>();
            _querySummaryRepository = Resolve<IRepository<QuerySummary, long>>();
        }

        private async Task InsertDataByQueryHistory()
        {
            await UsingDbContextAsync(async c =>
            {
                var history = await _queryHistoryRepository.InsertAsync(new QueryHistory
                {
                    UserId = GetHostAdmin().Id,
                    MainUsername = "a_user",
                    QueryWorkerHistories = new List<QueryWorkerHistory>
                    {
                        new QueryWorkerHistory
                        {
                            Solved = 3,
                            Submission = 10,
                            Username = "u1",
                            CrawlerName = "crawler",
                        },
                    },
                });

                await _querySummaryRepository.InsertAsync(new QuerySummary
                {
                    QueryHistoryId = history.Id,
                    Solved = 0,
                    Submission = 0,
                    QueryCrawlerSummaries = new List<QueryCrawlerSummary>
                    {
                        new QueryCrawlerSummary
                        {
                            Solved = 0,
                            Submission = 0,
                            Usernames = new List<UsernameInCrawler>
                            {
                                new UsernameInCrawler
                                {
                                    Username = "a_user",
                                    FromCrawlerName = "",
                                },
                            },
                            CrawlerName = "crawler",
                        },
                    },
                    SummaryWarnings = new List<SummaryWarning>
                    {
                        new SummaryWarning("c1", "a warning"),
                    },
                });
            });
        }

        private async Task InsertDataByQuerySummary()
        {
            await UsingDbContextAsync(async c =>
            {
                await _querySummaryRepository.InsertAsync(new QuerySummary
                {
                    QueryHistory = new QueryHistory
                    {
                        UserId = GetHostAdmin().Id,
                        MainUsername = "a_user",
                        QueryWorkerHistories = new List<QueryWorkerHistory>()
                        {
                            new QueryWorkerHistory
                            {
                                Solved = 3,
                                Submission = 10,
                                Username = "u1",
                                CrawlerName = "crawler",
                            },
                        },
                    },
                    Solved = 0,
                    Submission = 0,
                    QueryCrawlerSummaries = new List<QueryCrawlerSummary>
                    {
                        new QueryCrawlerSummary
                        {
                            Solved = 0,
                            Submission = 0,
                            Usernames = new List<UsernameInCrawler>
                            {
                                new UsernameInCrawler
                                {
                                    Username = "a_user",
                                    FromCrawlerName = "",
                                },
                            },
                            CrawlerName = "crawler",
                        },
                    },
                    SummaryWarnings = new List<SummaryWarning>
                    {
                        new SummaryWarning("c1", "a warning"),
                    },
                });
            });
        }

        [Theory]
        [InlineData("insert QueryHistory first")]
        [InlineData("insert QuerySummary directly")]
        public async Task AfterInsertion_ShouldSetForeignKey(string strategy)
        {
            // act
            if (strategy.Contains("QueryHistory"))
            {
                await InsertDataByQueryHistory();
            }
            else
            {
                await InsertDataByQuerySummary();
            }

            // test
            UsingDbContext(c =>
            {
                c.QueryHistories.Should().HaveCount(1);
                c.QueryWorkerHistories.Should().HaveCount(1);
                c.QuerySummaries.Should().HaveCount(1);
                c.UsernameInCrawler.Should().HaveCount(1);

                var queryHistory = c.QueryHistories.Single();
                var querySummary = c.QuerySummaries.Single();
                var queryCrawlerSummary = c.QueryCrawlerSummaries.Single();
                var queryWorkerHistory = c.QueryWorkerHistories.Single();

                querySummary.QueryHistoryId.Should().Be(queryHistory.Id);
                queryCrawlerSummary.QuerySummaryId.Should().Be(querySummary.Id);
                queryWorkerHistory.QueryHistoryId.Should().Be(queryHistory.Id);

                queryWorkerHistory.SolvedList.Should().BeNull();
                queryWorkerHistory.SubmissionsByCrawlerName.Should().BeNull();
            });
        }

        [Fact]
        public async Task WhenDeleteQuerySummary_ShouldNotDeleteQueryHistory()
        {
            // arrange
            await InsertDataByQuerySummary();

            // act
            UsingDbContext(c =>
            {
                var summary = c.QuerySummaries.Single();
                c.QuerySummaries.Remove(summary);
            });

            // assert
            UsingDbContext(c =>
            {
                c.QueryHistories.Should().HaveCount(1);
                c.QuerySummaries.Should().HaveCount(0);
                c.UsernameInCrawler.Should().HaveCount(0);
            });
        }

        [Fact]
        public async Task WhenDeleteQueryHistory_ShouldDeleteQuerySummary()
        {
            // arrange
            await InsertDataByQuerySummary();

            // act
            UsingDbContext(c =>
            {
                var history = c.QueryHistories.Single();
                c.QueryHistories.Remove(history);
            });

            // assert
            UsingDbContext(c =>
            {
                c.QueryHistories.Should().HaveCount(0);
                c.QuerySummaries.Should().HaveCount(0);
                c.UsernameInCrawler.Should().HaveCount(0);
            });
        }
    }
}
