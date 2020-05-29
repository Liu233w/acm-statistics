using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Abp.Application.Services.Dto;
using AcmStatisticsBackend.Crawlers;
using AcmStatisticsBackend.Crawlers.Dto;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace AcmStatisticsBackend.Tests.Crawlers
{
    public class QueryHistoryAppService_Tests : AcmStatisticsBackendTestBase
    {
        private readonly IQueryHistoryAppService _queryHistoryAppService;

        private readonly TestClockProvider _testClockProvider;

        public QueryHistoryAppService_Tests()
        {
            _testClockProvider = new TestClockProvider();
            _queryHistoryAppService = Resolve<QueryHistoryAppService>(new
            {
                clockProvider = _testClockProvider,
            });
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
                (await ctx.QueryHistories.CountAsync()).Should().Be(1);
                var acHistory = await ctx.QueryHistories.FirstAsync();
                Debug.Assert(AbpSession.UserId != null, "AbpSession.UserId != null");
                acHistory.UserId.Should().Be(AbpSession.UserId.Value);
                acHistory.CreationTime.Should().Be(new DateTime(2020, 4, 1, 10, 0, 0));
                acHistory.MainUsername.Should().Be("mainUser");

                (await ctx.QueryWorkerHistories.CountAsync()).Should().Be(2);
                var list = await ctx.QueryWorkerHistories.ToListAsync();
                list[0].WithIn(it =>
                {
                    it.Solved.Should().Be(3);
                    it.Submission.Should().Be(20);
                    it.Username.Should().Be("u1");
                    it.CrawlerName.Should().Be("c1");
                    it.QueryHistoryId.Should().Be(acHistory.Id);
                    it.ErrorMessage.Should().Be(null);
                    it.HasSolvedList.Should().Be(true);
                    it.SolvedList.Should().BeEquivalentTo(new[]
                    {
                        "p1",
                        "p2",
                        "p3",
                    });
                });
                list[1].WithIn(it =>
                {
                    it.Solved.Should().Be(0);
                    it.HasSolvedList.Should().BeFalse();
                    it.SolvedList.Should().BeEquivalentTo(new string[0]);
                    it.ErrorMessage.Should().Be("Cannot find username");
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
                (await ctx.QueryHistories.CountAsync()).Should().Be(1);
                var history = await ctx.QueryHistories.FirstAsync();

                (await ctx.QueryWorkerHistories.CountAsync()).Should().Be(1);
                var workerHistory = await ctx.QueryWorkerHistories.FirstAsync();
                workerHistory.QueryHistoryId.Should().Be(history.Id);
                workerHistory.Solved.Should().Be(5);
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
                (await ctx.QueryHistories.CountAsync()).Should().Be(2);
                (await ctx.QueryWorkerHistories.CountAsync()).Should().Be(2);
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
                (await ctx.QueryHistories.CountAsync()).Should().Be(0);
                (await ctx.QueryWorkerHistories.CountAsync()).Should().Be(0);
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
            result.TotalCount.Should().Be(1);
            result.Items[0].WithIn(it =>
            {
                it.MainUsername.Should().Be("u1");
                it.CreationTime.Should().Be(new DateTime(2020, 4, 1, 10, 0, 0));
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
                        MainUsername = "u1",
                        CreationTime = new DateTime(2020, 4, 1, 10, 0, 0),
                        UserId = AbpSession.UserId.Value,
                    },
                    new QueryHistory
                    {
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
            result.TotalCount.Should().Be(2);
            result.Items[0].WithIn(it =>
            {
                it.MainUsername.Should().Be("u1");
                it.CreationTime.Should().Be(new DateTime(2020, 4, 2, 10, 0, 0));
            });
            result.Items[1].WithIn(it =>
            {
                it.MainUsername.Should().Be("u1");
                it.CreationTime.Should().Be(new DateTime(2020, 4, 1, 10, 0, 0));
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
            list.Items.Count.Should().Be(2);
            list.Items[0].WithIn(it =>
            {
                it.Solved.Should().Be(3);
                it.Submission.Should().Be(20);
                it.Username.Should().Be("u1");
                it.CrawlerName.Should().Be("c1");
                it.ErrorMessage.Should().BeNull();
                it.HasSolvedList.Should().Be(true);
                it.SolvedList.Should().BeEquivalentTo(new string[]
                {
                    "p1",
                    "p2",
                    "p3",
                });
            });
            list.Items[1].WithIn(it =>
            {
                it.Solved.Should().Be(0);
                it.Username.Should().Be("u2");
                it.CrawlerName.Should().Be("c2");
                it.ErrorMessage.Should().Be("Cannot find username");
                it.HasSolvedList.Should().Be(false);
                it.SolvedList.Should().BeEquivalentTo(new string[0]);
            });
        }

        [Fact]
        public async Task It_CanSaveAndGetRecordWithSubmissions()
        {
            // arrange
            LoginAsDefaultTenantAdmin();
            _testClockProvider.Now = new DateTime(2020, 4, 1, 10, 0, 0);

            // act
            await _queryHistoryAppService.SaveOrReplaceQueryHistory(new SaveOrReplaceQueryHistoryInput
            {
                MainUsername = "mainUser",
                QueryWorkerHistories = new List<QueryWorkerHistoryDto>
                {
                    new QueryWorkerHistoryDto
                    {
                        Username = "u1",
                        CrawlerName = "c1",
                        Solved = 3,
                        Submission = 10,
                        ErrorMessage = null,
                        HasSolvedList = true,
                        SolvedList = new[]
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
                        Solved = 3,
                        Submission = 10,
                        HasSolvedList = true,
                        SolvedList = new[]
                        {
                            "c1-p1",
                            "c1-p5",
                            "NO_NAME-1001",
                        },
                        IsVirtualJudge = true,
                        SubmissionsByCrawlerName = new Dictionary<string, int>
                        {
                            { "c1", 5 },
                            { "NO_NAME", 5 },
                        },
                    },
                },
            });

            // assert
            await UsingDbContextAsync(async ctx =>
            {
                (await ctx.QueryHistories.CountAsync()).Should().Be(1);
                var acHistory = await ctx.QueryHistories.FirstAsync();
                Debug.Assert(AbpSession.UserId != null, "AbpSession.UserId != null");
                acHistory.UserId.Should().Be(AbpSession.UserId.Value);
                acHistory.CreationTime.Should().Be(new DateTime(2020, 4, 1, 10, 0, 0));
                acHistory.MainUsername.Should().Be("mainUser");

                (await ctx.QueryWorkerHistories.CountAsync()).Should().Be(2);
                var list = await ctx.QueryWorkerHistories.ToListAsync();
                list[0].WithIn(it =>
                {
                    it.Solved.Should().Be(3);
                    it.Submission.Should().Be(10);
                    it.Username.Should().Be("u1");
                    it.CrawlerName.Should().Be("c1");
                    it.QueryHistoryId.Should().Be(acHistory.Id);
                    it.ErrorMessage.Should().Be(null);
                    it.HasSolvedList.Should().Be(true);
                    it.SolvedList.Should().BeEquivalentTo(new string[]
                    {
                        "p1",
                        "p2",
                        "p3",
                    });
                });
                list[1].WithIn(it =>
                {
                    it.Solved.Should().Be(3);
                    it.HasSolvedList.Should().Be(true);
                    it.SolvedList.Should().BeEquivalentTo(new string[]
                    {
                        "c1-p1",
                        "c1-p5",
                        "NO_NAME-1001",
                    });
                    it.IsVirtualJudge.Should().Be(true);
                    it.SubmissionsByCrawlerName.Should().BeEquivalentTo(new Dictionary<string, int>
                    {
                        { "c1", 5 },
                        { "NO_NAME", 5 },
                    });
                });
            });
        }
    }
}
