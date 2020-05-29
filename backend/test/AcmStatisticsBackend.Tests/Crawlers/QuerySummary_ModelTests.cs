using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using AcmStatisticsBackend.Crawlers;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace AcmStatisticsBackend.Tests.Crawlers
{
    public class QuerySummary_ModelTests : AcmStatisticsBackendTestBase
    {
        private readonly IRepository<QueryHistory, long> _queryHistoryRepository;

        public QuerySummary_ModelTests()
        {
            _queryHistoryRepository = Resolve<IRepository<QueryHistory, long>>();
        }

        private async Task InsertData()
        {
            await UsingDbContextAsync(async c =>
            {
                await _queryHistoryRepository.InsertAsync(new QueryHistory
                {
                    UserId = GetHostAdmin().Id,
                    QuerySummary = new QuerySummary
                    {
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
                    },
                    MainUsername = "a_user",
                    QueryWorkerHistories = new List<QueryWorkerHistory>(),
                });

                (await c.QueryHistories.CountAsync()).Should().Be(1);
                (await c.QuerySummaries.CountAsync()).Should().Be(1);
                (await c.UsernameInCrawler.CountAsync()).Should().Be(1);
            });
        }

        [Fact]
        public async Task WhenDeleteQuerySummary_ShouldSetFieldInQueryHistoryToNull()
        {
            // arrange
            await InsertData();

            // act
            await UsingDbContextAsync(async c =>
            {
                var summary = await c.QuerySummaries.FirstAsync();
                c.QuerySummaries.Remove(summary);
            });

            // assert
            await UsingDbContextAsync(async c =>
            {
                (await c.QueryHistories.CountAsync()).Should().Be(1);
                (await c.QuerySummaries.CountAsync()).Should().Be(0);
                (await c.UsernameInCrawler.CountAsync()).Should().Be(0);

                var history = await c.QueryHistories.FirstAsync();
                history.QuerySummaryId.Should().BeNull();
            });
        }

        [Fact]
        public async Task WhenDeleteQueryHistory_ShouldDeleteQuerySummary()
        {
            // arrange
            await InsertData();

            // act
            await UsingDbContextAsync(async c =>
            {
                var history = await c.QueryHistories.FirstAsync();
                c.QueryHistories.Remove(history);
            });

            // assert
            await UsingDbContextAsync(async c =>
            {
                (await c.QueryHistories.CountAsync()).Should().Be(0);
                (await c.QuerySummaries.CountAsync()).Should().Be(0);
                (await c.UsernameInCrawler.CountAsync()).Should().Be(0);
            });
        }
    }
}
