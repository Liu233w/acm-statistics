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
    public class AcHistoryAppService_Tests : AcmStatisticsBackendTestBase
    {
        private readonly IAcHistoryAppService _acHistoryAppService;

        private readonly TestClockProvider _testClockProvider;

        public AcHistoryAppService_Tests()
        {
            _acHistoryAppService = Resolve<AcHistoryAppService>();
            _testClockProvider = new TestClockProvider();
            Clock.Provider = _testClockProvider;
        }

        [Fact]
        public async Task SaveOrReplaceAcHistory_能够存储记录()
        {
            // arrange
            LoginAsDefaultTenantAdmin();
            _testClockProvider.Now = new DateTime(2020, 4, 1, 10, 0, 0);

            // act
            await _acHistoryAppService.SaveOrReplaceAcHistory(new SaveOrReplaceAcHistoryInput
            {
                Solved = 3,
                Submission = 20,
                MainUsername = "mainUser",
                AcWorkerHistories = new List<AcWorkerHistoryDto>
                {
                    new AcWorkerHistoryDto
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
                    new AcWorkerHistoryDto
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
                (await ctx.AcHistories.CountAsync()).ShouldBe(1);
                var acHistory = await ctx.AcHistories.FirstAsync();
                acHistory.Solved.ShouldBe(3);
                acHistory.Submission.ShouldBe(20);
                Debug.Assert(AbpSession.UserId != null, "AbpSession.UserId != null");
                acHistory.UserId.ShouldBe(AbpSession.UserId.Value);
                acHistory.CreationTime.ShouldBe(new DateTime(2020, 4, 1, 10, 0, 0));
                acHistory.MainUsername.ShouldBe("mainUser");

                (await ctx.AcWorkerHistories.CountAsync()).ShouldBe(2);
                var list = await ctx.AcWorkerHistories.ToListAsync();
                list[0].WithIn(it =>
                {
                    it.Solved.ShouldBe(3);
                    it.Submission.ShouldBe(20);
                    it.Username.ShouldBe("u1");
                    it.CrawlerName.ShouldBe("c1");
                    it.AcHistoryId.ShouldBe(acHistory.Id);
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
        public async Task SaveOrReplaceAcHistory_能够顶掉同一天的记录()
        {
            // arrange
            LoginAsDefaultTenantAdmin();

            // act
            _testClockProvider.Now = new DateTime(2020, 4, 1, 10, 0, 0);
            await _acHistoryAppService.SaveOrReplaceAcHistory(new SaveOrReplaceAcHistoryInput
            {
                Solved = 1,
                Submission = 10,
                MainUsername = "u1",
                AcWorkerHistories = new List<AcWorkerHistoryDto>
                {
                    new AcWorkerHistoryDto
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
            await _acHistoryAppService.SaveOrReplaceAcHistory(new SaveOrReplaceAcHistoryInput
            {
                Solved = 5,
                Submission = 10,
                MainUsername = "u1",
                AcWorkerHistories = new List<AcWorkerHistoryDto>
                {
                    new AcWorkerHistoryDto
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
                (await ctx.AcHistories.CountAsync()).ShouldBe(1);
                var history = await ctx.AcHistories.FirstAsync();
                history.Solved.ShouldBe(5);

                (await ctx.AcWorkerHistories.CountAsync()).ShouldBe(1);
                var workerHistory = await ctx.AcWorkerHistories.FirstAsync();
                workerHistory.AcHistoryId.ShouldBe(history.Id);
                workerHistory.Solved.ShouldBe(5);
            });
        }

        [Fact]
        public async Task SaveOrReplaceAcHistory_能够保留不同天的记录()
        {
            // arrange
            LoginAsDefaultTenantAdmin();

            // act
            _testClockProvider.Now = new DateTime(2020, 4, 1, 10, 0, 0);
            await _acHistoryAppService.SaveOrReplaceAcHistory(new SaveOrReplaceAcHistoryInput
            {
                Solved = 1,
                Submission = 10,
                MainUsername = "u1",
                AcWorkerHistories = new List<AcWorkerHistoryDto>
                {
                    new AcWorkerHistoryDto
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
            await _acHistoryAppService.SaveOrReplaceAcHistory(new SaveOrReplaceAcHistoryInput
            {
                Solved = 5,
                Submission = 10,
                MainUsername = "u1",
                AcWorkerHistories = new List<AcWorkerHistoryDto>
                {
                    new AcWorkerHistoryDto
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
                (await ctx.AcHistories.CountAsync()).ShouldBe(2);
                (await ctx.AcWorkerHistories.CountAsync()).ShouldBe(2);
            });
        }

        [Fact]
        public async Task DeleteAcHistory_能够移除AcHistory和AcWorkerHistory()
        {
            // arrange
            LoginAsDefaultTenantAdmin();
            _testClockProvider.Now = new DateTime(2020, 4, 1, 10, 0, 0);
            await _acHistoryAppService.SaveOrReplaceAcHistory(new SaveOrReplaceAcHistoryInput
            {
                Solved = 1,
                Submission = 10,
                MainUsername = "u1",
                AcWorkerHistories = new List<AcWorkerHistoryDto>
                {
                    new AcWorkerHistoryDto
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
            await _acHistoryAppService.DeleteAcHistory(new DeleteAcHistoryInput
            {
                Id = 1,
            });

            // assert
            await UsingDbContextAsync(async ctx =>
            {
                (await ctx.AcHistories.CountAsync()).ShouldBe(0);
                (await ctx.AcWorkerHistories.CountAsync()).ShouldBe(0);
            });
        }

        [Fact]
        public async Task GetAcHistory_能够正确获取数据()
        {
            // arrange
            LoginAsDefaultTenantAdmin();
            await UsingDbContextAsync(async ctx =>
            {
                Debug.Assert(AbpSession.UserId != null, "AbpSession.UserId != null");
                await ctx.AcHistories.AddAsync(new AcHistory
                {
                    Solved = 1,
                    Submission = 10,
                    MainUsername = "u1",
                    CreationTime = new DateTime(2020, 4, 1, 10, 0, 0),
                    UserId = AbpSession.UserId.Value,
                });
            });

            // act
            var result = await _acHistoryAppService.GetAcHistory(new PagedResultRequestDto
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
        public async Task GetAcHistory_能够按照正确顺序获取数据()
        {
            // arrange
            LoginAsDefaultTenantAdmin();
            await UsingDbContextAsync(async ctx =>
            {
                Debug.Assert(AbpSession.UserId != null, "AbpSession.UserId != null");
                await ctx.AcHistories.AddRangeAsync(new[]
                {
                    new AcHistory
                    {
                        Solved = 1,
                        Submission = 10,
                        MainUsername = "u1",
                        CreationTime = new DateTime(2020, 4, 1, 10, 0, 0),
                        UserId = AbpSession.UserId.Value,
                    },
                    new AcHistory
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
            var result = await _acHistoryAppService.GetAcHistory(new PagedResultRequestDto
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
        public async Task GetAcWorkerHistory_能够获取正确记录()
        {
            // arrange
            LoginAsDefaultTenantAdmin();
            _testClockProvider.Now = new DateTime(2020, 4, 1, 10, 0, 0);
            await _acHistoryAppService.SaveOrReplaceAcHistory(new SaveOrReplaceAcHistoryInput
            {
                Solved = 3,
                Submission = 20,
                MainUsername = "mainUser",
                AcWorkerHistories = new List<AcWorkerHistoryDto>
                {
                    new AcWorkerHistoryDto
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
                    new AcWorkerHistoryDto
                    {
                        Username = "u2",
                        CrawlerName = "c2",
                        ErrorMessage = "Cannot find username",
                    },
                },
            });

            var historyResponse = await _acHistoryAppService.GetAcHistory(new PagedResultRequestDto());
            var historyId = historyResponse.Items[0].Id;

            // act
            var list = await _acHistoryAppService.GetAcWorkerHistory(new GetAcWorkerHistoryInput
            {
                AcHistoryId = historyId,
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
