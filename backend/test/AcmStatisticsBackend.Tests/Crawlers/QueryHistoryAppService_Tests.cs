using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Abp.Application.Services.Dto;
using Abp.Timing;
using AcmStatisticsBackend.Crawlers;
using AcmStatisticsBackend.Crawlers.Dto;
using Microsoft.EntityFrameworkCore;
using Shouldly;
using Xunit;

namespace AcmStatisticsBackend.Tests.Crawlers
{
    public class QueryHistoryAppService_Tests : AcmStatisticsBackendTestBase
    {
        private readonly IQueryHistoryAppService _queryHistoryAppService;

        private readonly TestClockProvider _testClockProvider;

        public QueryHistoryAppService_Tests()
        {
            _queryHistoryAppService = Resolve<QueryHistoryAppService>();
            _testClockProvider = new TestClockProvider();
            Clock.Provider = _testClockProvider;
        }

        [Fact]
        public async Task SaveOrReplaceQueryHistory_CanSaveRecord()
        {
            // arrange
            LoginAsDefaultTenantAdmin();
            _testClockProvider.Now = new DateTime(2020, 4, 1, 10, 0, 0);

            // act
            await _queryHistoryAppService.SaveOrReplaceQueryHistory(new SaveOrReplaceQueryHistoryInput
            {
                Solved = 3,
                Submission = 20,
                MainUsername = "mainUser",
                QueryWorkerHistories = new List<QueryWorkerHistoryDto>
                {
                    new QueryWorkerHistoryDto
                    {
                        Username = "u1",
                        CrawlerName = "c1",
                        Solved = 3,
                        Submission = 20,
                        ErrorMessage = null,
                        HasSolvedList = true,
                        SolvedList = new string[]
                        {
                            "p1",
                            "p2",
                            "p3",
                        },
                    },
                    new QueryWorkerHistoryDto
                    {
                        Username = "u2",
                        CrawlerName = "c2",
                        ErrorMessage = "Cannot find username",
                    },
                },
            });

            // assert
            await UsingDbContextAsync(async ctx =>
            {
                (await ctx.QueryHistories.CountAsync()).ShouldBe(1);
                var acHistory = await ctx.QueryHistories.FirstAsync();
                acHistory.Solved.ShouldBe(3);
                acHistory.Submission.ShouldBe(20);
                Debug.Assert(AbpSession.UserId != null, "AbpSession.UserId != null");
                acHistory.UserId.ShouldBe(AbpSession.UserId.Value);
                acHistory.CreationTime.ShouldBe(new DateTime(2020, 4, 1, 10, 0, 0));
                acHistory.MainUsername.ShouldBe("mainUser");

                (await ctx.QueryWorkerHistories.CountAsync()).ShouldBe(2);
                var list = await ctx.QueryWorkerHistories.ToListAsync();
                list[0].WithIn(it =>
                {
                    it.Solved.ShouldBe(3);
                    it.Submission.ShouldBe(20);
                    it.Username.ShouldBe("u1");
                    it.CrawlerName.ShouldBe("c1");
                    it.QueryHistoryId.ShouldBe(acHistory.Id);
                    it.ErrorMessage.ShouldBe(null);
                    it.HasSolvedList.ShouldBe(true);
                    it.SolvedList.ShouldBe(new string[]
                    {
                        "p1",
                        "p2",
                        "p3",
                    });
                });
                list[1].WithIn(it =>
                {
                    it.Solved.ShouldBe(0);
                    it.HasSolvedList.ShouldBe(false);
                    it.SolvedList.ShouldBe(new string[0]);
                    it.ErrorMessage.ShouldBe("Cannot find username");
                });
            });
        }

        [Fact]
        public async Task SaveOrReplaceQueryHistory_CanReplaceRecordOfTheSameDay()
        {
            // arrange
            LoginAsDefaultTenantAdmin();

            // act
            _testClockProvider.Now = new DateTime(2020, 4, 1, 10, 0, 0);
            await _queryHistoryAppService.SaveOrReplaceQueryHistory(new SaveOrReplaceQueryHistoryInput
            {
                Solved = 1,
                Submission = 10,
                MainUsername = "u1",
                QueryWorkerHistories = new List<QueryWorkerHistoryDto>
                {
                    new QueryWorkerHistoryDto
                    {
                        CrawlerName = "c1",
                        Username = "u1",
                        Solved = 1,
                        Submission = 10,
                        HasSolvedList = false,
                    },
                },
            });

            // 第二次存储
            _testClockProvider.Now = new DateTime(2020, 4, 1, 20, 0, 0);
            await _queryHistoryAppService.SaveOrReplaceQueryHistory(new SaveOrReplaceQueryHistoryInput
            {
                Solved = 5,
                Submission = 10,
                MainUsername = "u1",
                QueryWorkerHistories = new List<QueryWorkerHistoryDto>
                {
                    new QueryWorkerHistoryDto
                    {
                        CrawlerName = "c1",
                        Username = "u1",
                        Solved = 5,
                        Submission = 10,
                        HasSolvedList = false,
                    },
                },
            });

            // assert
            await UsingDbContextAsync(async ctx =>
            {
                (await ctx.QueryHistories.CountAsync()).ShouldBe(1);
                var history = await ctx.QueryHistories.FirstAsync();
                history.Solved.ShouldBe(5);

                (await ctx.QueryWorkerHistories.CountAsync()).ShouldBe(1);
                var workerHistory = await ctx.QueryWorkerHistories.FirstAsync();
                workerHistory.QueryHistoryId.ShouldBe(history.Id);
                workerHistory.Solved.ShouldBe(5);
            });
        }

        [Fact]
        public async Task SaveOrReplaceAcHistory_ShouldKeepRecordsOfDifferentDays()
        {
            // arrange
            LoginAsDefaultTenantAdmin();

            // act
            _testClockProvider.Now = new DateTime(2020, 4, 1, 10, 0, 0);
            await _queryHistoryAppService.SaveOrReplaceQueryHistory(new SaveOrReplaceQueryHistoryInput
            {
                Solved = 1,
                Submission = 10,
                MainUsername = "u1",
                QueryWorkerHistories = new List<QueryWorkerHistoryDto>
                {
                    new QueryWorkerHistoryDto
                    {
                        CrawlerName = "c1",
                        Username = "u1",
                        Solved = 1,
                        Submission = 10,
                        HasSolvedList = false,
                    },
                },
            });

            // 第二次存储
            _testClockProvider.Now = new DateTime(2020, 4, 2, 10, 0, 0);
            await _queryHistoryAppService.SaveOrReplaceQueryHistory(new SaveOrReplaceQueryHistoryInput
            {
                Solved = 5,
                Submission = 10,
                MainUsername = "u1",
                QueryWorkerHistories = new List<QueryWorkerHistoryDto>
                {
                    new QueryWorkerHistoryDto
                    {
                        CrawlerName = "c1",
                        Username = "u1",
                        Solved = 5,
                        Submission = 10,
                        HasSolvedList = false,
                    },
                },
            });

            // assert
            await UsingDbContextAsync(async ctx =>
            {
                (await ctx.QueryHistories.CountAsync()).ShouldBe(2);
                (await ctx.QueryWorkerHistories.CountAsync()).ShouldBe(2);
            });
        }

        [Fact]
        public async Task DeleteAcHistory_ShouldRemoveAcHistoryAndAcWorkerHistoryAtTheSameTime()
        {
            // arrange
            LoginAsDefaultTenantAdmin();
            _testClockProvider.Now = new DateTime(2020, 4, 1, 10, 0, 0);
            await _queryHistoryAppService.SaveOrReplaceQueryHistory(new SaveOrReplaceQueryHistoryInput
            {
                Solved = 1,
                Submission = 10,
                MainUsername = "u1",
                QueryWorkerHistories = new List<QueryWorkerHistoryDto>
                {
                    new QueryWorkerHistoryDto
                    {
                        CrawlerName = "c1",
                        Username = "u1",
                        Solved = 1,
                        Submission = 10,
                        HasSolvedList = false,
                    },
                },
            });

            // act
            await _queryHistoryAppService.DeleteQueryHistory(new DeleteQueryHistoryInput
            {
                Id = 1,
            });

            // assert
            await UsingDbContextAsync(async ctx =>
            {
                (await ctx.QueryHistories.CountAsync()).ShouldBe(0);
                (await ctx.QueryWorkerHistories.CountAsync()).ShouldBe(0);
            });
        }

        [Fact]
        public async Task GetAcHistory_ShouldWorkCorrectly()
        {
            // arrange
            LoginAsDefaultTenantAdmin();
            await UsingDbContextAsync(async ctx =>
            {
                Debug.Assert(AbpSession.UserId != null, "AbpSession.UserId != null");
                await ctx.QueryHistories.AddAsync(new QueryHistory
                {
                    Solved = 1,
                    Submission = 10,
                    MainUsername = "u1",
                    CreationTime = new DateTime(2020, 4, 1, 10, 0, 0),
                    UserId = AbpSession.UserId.Value,
                });
            });

            // act
            var result = await _queryHistoryAppService.GetQueryHistories(new PagedResultRequestDto
            {
                SkipCount = 0,
                MaxResultCount = 5,
            });

            // assert
            result.TotalCount.ShouldBe(1);
            result.Items[0].WithIn(it =>
            {
                it.Solved.ShouldBe(1);
                it.Submission.ShouldBe(10);
                it.MainUsername.ShouldBe("u1");
                it.CreationTime.ShouldBe(new DateTime(2020, 4, 1, 10, 0, 0));
            });
        }

        [Fact]
        public async Task GetAcHistory_ShouldReturnHistoriesInCorrectOrder()
        {
            // arrange
            LoginAsDefaultTenantAdmin();
            await UsingDbContextAsync(async ctx =>
            {
                Debug.Assert(AbpSession.UserId != null, "AbpSession.UserId != null");
                await ctx.QueryHistories.AddRangeAsync(new[]
                {
                    new QueryHistory
                    {
                        Solved = 1,
                        Submission = 10,
                        MainUsername = "u1",
                        CreationTime = new DateTime(2020, 4, 1, 10, 0, 0),
                        UserId = AbpSession.UserId.Value,
                    },
                    new QueryHistory
                    {
                        Solved = 2,
                        Submission = 11,
                        MainUsername = "u1",
                        CreationTime = new DateTime(2020, 4, 2, 10, 0, 0),
                        UserId = AbpSession.UserId.Value,
                    },
                });
            });

            // act
            var result = await _queryHistoryAppService.GetQueryHistories(new PagedResultRequestDto
            {
                SkipCount = 0,
                MaxResultCount = 5,
            });

            // assert
            result.TotalCount.ShouldBe(2);
            result.Items[0].WithIn(it =>
            {
                it.Solved.ShouldBe(2);
                it.Submission.ShouldBe(11);
                it.MainUsername.ShouldBe("u1");
                it.CreationTime.ShouldBe(new DateTime(2020, 4, 2, 10, 0, 0));
            });
            result.Items[1].WithIn(it =>
            {
                it.Solved.ShouldBe(1);
                it.Submission.ShouldBe(10);
                it.MainUsername.ShouldBe("u1");
                it.CreationTime.ShouldBe(new DateTime(2020, 4, 1, 10, 0, 0));
            });
        }

        [Fact]
        public async Task GetAcWorkerHistory_ShouldWorkCorrectly()
        {
            // arrange
            LoginAsDefaultTenantAdmin();
            _testClockProvider.Now = new DateTime(2020, 4, 1, 10, 0, 0);
            await _queryHistoryAppService.SaveOrReplaceQueryHistory(new SaveOrReplaceQueryHistoryInput
            {
                Solved = 3,
                Submission = 20,
                MainUsername = "mainUser",
                QueryWorkerHistories = new List<QueryWorkerHistoryDto>
                {
                    new QueryWorkerHistoryDto
                    {
                        Username = "u1",
                        CrawlerName = "c1",
                        Solved = 3,
                        Submission = 20,
                        ErrorMessage = null,
                        HasSolvedList = true,
                        SolvedList = new string[]
                        {
                            "p1",
                            "p2",
                            "p3",
                        },
                    },
                    new QueryWorkerHistoryDto
                    {
                        Username = "u2",
                        CrawlerName = "c2",
                        ErrorMessage = "Cannot find username",
                    },
                },
            });

            var historyResponse = await _queryHistoryAppService.GetQueryHistories(new PagedResultRequestDto());
            var historyId = historyResponse.Items[0].Id;

            // act
            var list = await _queryHistoryAppService.GetQueryWorkerHistories(new GetAcWorkerHistoryInput
            {
                QueryHistoryId = historyId,
            });

            // assert
            list.Items.Count.ShouldBe(2);
            list.Items[0].WithIn(it =>
            {
                it.Solved.ShouldBe(3);
                it.Submission.ShouldBe(20);
                it.Username.ShouldBe("u1");
                it.CrawlerName.ShouldBe("c1");
                it.ErrorMessage.ShouldBeNull();
                it.HasSolvedList.ShouldBe(true);
                it.SolvedList.ShouldEqualInJson(new string[]
                {
                    "p1",
                    "p2",
                    "p3",
                });
            });
            list.Items[1].WithIn(it =>
            {
                it.Solved.ShouldBe(0);
                it.Username.ShouldBe("u2");
                it.CrawlerName.ShouldBe("c2");
                it.ErrorMessage.ShouldBe("Cannot find username");
                it.HasSolvedList.ShouldBe(false);
                it.SolvedList.ShouldEqualInJson(new string[0]);
            });
        }
    }

    internal class TestClockProvider : IClockProvider
    {
        public DateTime Normalize(DateTime dateTime)
        {
            return ClockProviders.Utc.Normalize(dateTime);
        }

        public DateTime Now { get; set; }

        public DateTimeKind Kind => DateTimeKind.Utc;

        public bool SupportsMultipleTimezone => true;
    }
}
