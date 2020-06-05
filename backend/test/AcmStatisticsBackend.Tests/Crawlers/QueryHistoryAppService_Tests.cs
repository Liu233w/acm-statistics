using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Abp.Application.Services.Dto;
using Abp.Timing;
using Abp.UI;
using AcmStatisticsBackend.Crawlers;
using AcmStatisticsBackend.Crawlers.Dto;
using AcmStatisticsBackend.ServiceClients;
using AcmStatisticsBackend.Tests.DependencyInjection;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace AcmStatisticsBackend.Tests.Crawlers
{
    public class QueryHistoryAppService_Tests : AcmStatisticsBackendTestBase
    {
        private IQueryHistoryAppService _queryHistoryAppService;

        private TestClockProvider _testClockProvider;

        protected override void PreInitialize()
        {
            LocalIocManager.Register<IClockProvider, TestClockProvider>();
        }

        protected override void PostInitialize()
        {
            _testClockProvider = Resolve<IClockProvider>() as TestClockProvider;

            var testCrawlerApiBackendClient = new TestCrawlerApiBackendClient();
            _queryHistoryAppService = Resolve<QueryHistoryAppService>(new
            {
                crawlerApiBackendClient = testCrawlerApiBackendClient,
            });

            testCrawlerApiBackendClient.CrawlerMeta = new List<CrawlerMetaItem>
            {
                new CrawlerMetaItem
                {
                    CrawlerName = "c1",
                    CrawlerTitle = "C1",
                    Url = "",
                    CrawlerDescription = "",
                    IsVirtualJudge = false,
                },
                new CrawlerMetaItem
                {
                    CrawlerName = "c2",
                    CrawlerTitle = "C2",
                    Url = "",
                    CrawlerDescription = "",
                    IsVirtualJudge = false,
                },
                new CrawlerMetaItem
                {
                    CrawlerName = "c3",
                    CrawlerTitle = "C3",
                    Url = "",
                    CrawlerDescription = "",
                    IsVirtualJudge = true,
                },
                new CrawlerMetaItem
                {
                    CrawlerName = "c4",
                    CrawlerTitle = "C4",
                    Url = "",
                    CrawlerDescription = "",
                    IsVirtualJudge = true,
                },
            };
        }

        [Fact]
        public async Task SaveOrReplaceQueryHistory_CanSaveRecord()
        {
            // arrange
            LoginAsDefaultTenantAdmin();
            _testClockProvider.Now = new DateTime(2020, 4, 1, 10, 0, 0);

            // act
            var result = await _queryHistoryAppService.SaveOrReplaceQueryHistory(new SaveOrReplaceQueryHistoryInput
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
                        ErrorMessage = "Cannot find username",
                    },
                },
            });

            // assert
            UsingDbContext(ctx =>
            {
                ctx.QueryHistories.Should().HaveCount(1);
                var acHistory = ctx.QueryHistories.Single();
                Debug.Assert(AbpSession.UserId != null, "AbpSession.UserId != null");
                acHistory.UserId.Should().Be(AbpSession.UserId.Value);
                acHistory.CreationTime.Should().Be(new DateTime(2020, 4, 1, 10, 0, 0));
                acHistory.MainUsername.Should().Be("mainUser");
                acHistory.IsReliableSource.Should().BeFalse();

                ctx.QueryWorkerHistories.Should().HaveCount(2);
                var list = ctx.QueryWorkerHistories.ToList();
                list[0].WithIn(it =>
                {
                    it.Solved.Should().Be(3);
                    it.Submission.Should().Be(20);
                    it.Username.Should().Be("u1");
                    it.CrawlerName.Should().Be("c1");
                    it.QueryHistoryId.Should().Be(acHistory.Id);
                    it.ErrorMessage.Should().Be(null);
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
                    it.SolvedList.Should().BeNull();
                    it.ErrorMessage.Should().Be("Cannot find username");
                });
            });

            result.QueryHistoryId.Should().Be(1);
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
                    },
                },
            });

            // 第二次存储
            _testClockProvider.Now = new DateTime(2020, 4, 1, 20, 0, 0);
            var result = await _queryHistoryAppService.SaveOrReplaceQueryHistory(new SaveOrReplaceQueryHistoryInput
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
                    },
                },
            });

            // assert
            UsingDbContext(ctx =>
            {
                ctx.QueryHistories.Should().HaveCount(1);
                var history = ctx.QueryHistories.Single();

                ctx.QueryWorkerHistories.Should().HaveCount(1);
                var workerHistory = ctx.QueryWorkerHistories.Single();
                workerHistory.QueryHistoryId.Should().Be(history.Id);
                workerHistory.Solved.Should().Be(5);
            });

            result.QueryHistoryId.Should().Be(2);
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
                    },
                },
            });

            // 第二次存储
            _testClockProvider.Now = new DateTime(2020, 4, 2, 10, 0, 0);
            var result = await _queryHistoryAppService.SaveOrReplaceQueryHistory(new SaveOrReplaceQueryHistoryInput
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
                    },
                },
            });

            // assert
            UsingDbContext(ctx =>
            {
                ctx.QueryHistories.Should().HaveCount(2);
                ctx.QueryWorkerHistories.Should().HaveCount(2);
            });

            result.QueryHistoryId.Should().Be(2);
        }

        [Fact]
        public async Task SaveOrReplaceQueryHistory_ShouldGenerateSummary()
        {
            // arrange
            LoginAsDefaultTenantAdmin();
            _testClockProvider.Now = new DateTime(2020, 4, 1, 10, 0, 0);

            // act
            var result = await _queryHistoryAppService.SaveOrReplaceQueryHistory(new SaveOrReplaceQueryHistoryInput
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
                        ErrorMessage = "Cannot find username",
                    },
                },
            });

            // assert
            UsingDbContext(ctx =>
            {
                ctx.QuerySummaries.Should().HaveCount(1);
                var querySummary = ctx.QuerySummaries.Single();
                querySummary.WithIn(it =>
                {
                    it.Solved.Should().Be(3);
                    it.Submission.Should().Be(20);
                    it.QueryHistoryId.Should().Be(result.QueryHistoryId);
                });

                ctx.QueryCrawlerSummaries.Should().HaveCount(1);
                ctx.QueryCrawlerSummaries.Single().WithIn(it => { it.QuerySummaryId.Should().Be(querySummary.Id); });
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
                    },
                },
            });

            // act
            await _queryHistoryAppService.DeleteQueryHistory(new DeleteQueryHistoryInput
            {
                Id = 1,
            });

            // assert
            UsingDbContext(ctx =>
            {
                ctx.QueryHistories.Should().HaveCount(0);
                ctx.QueryWorkerHistories.Should().HaveCount(0);
                ctx.QuerySummaries.Should().HaveCount(0);
                ctx.QueryCrawlerSummaries.Should().HaveCount(0);
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
                        SolvedList = new[]
                        {
                            "p1",
                            "p2",
                            "p3",
                        },
                        IsVirtualJudge = false,
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
                it.SolvedList.Should().BeEquivalentTo(new[]
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
                it.SolvedList.Should().BeNull();
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
                        CrawlerName = "c3",
                        Solved = 3,
                        Submission = 10,
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

        [Fact]
        public async Task GetQuerySummary_ShouldWorkCorrectly()
        {
            // arrange
            LoginAsDefaultTenantAdmin();
            _testClockProvider.Now = new DateTime(2020, 4, 1, 10, 0, 0);
            var saveOrReplaceQueryHistoryOutput = await _queryHistoryAppService.SaveOrReplaceQueryHistory(
                new SaveOrReplaceQueryHistoryInput
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
                            ErrorMessage = "Cannot find username",
                        },
                    },
                });

            // act
            var result = await _queryHistoryAppService.GetQuerySummary(new GetQuerySummaryInput
            {
                QueryHistoryId = saveOrReplaceQueryHistoryOutput.QueryHistoryId,
            });

            // assert
            result.Should().BeEquivalentTo(new QuerySummaryDto
            {
                QueryHistoryId = saveOrReplaceQueryHistoryOutput.QueryHistoryId,
                GenerateTime = _testClockProvider.Now,
                MainUsername = "mainUser",
                Solved = 3,
                Submission = 20,
                SummaryWarnings = new List<SummaryWarning>(),
                QueryCrawlerSummaries = new List<QueryCrawlerSummaryDto>
                {
                    new QueryCrawlerSummaryDto
                    {
                        CrawlerName = "c1",
                        Usernames = new List<UsernameInCrawlerDto>
                        {
                            new UsernameInCrawlerDto
                            {
                                Username = "u1",
                            },
                        },
                        Solved = 3,
                        Submission = 20,
                        IsVirtualJudge = false,
                    },
                },
            });
        }

        [Fact]
        public async Task GetQuerySummary_WhenSummaryNotExist_ShouldThrow()
        {
            // arrange
            LoginAsDefaultTenantAdmin();
            var user = await GetCurrentUserAsync();

            var id = UsingDbContext(c =>
            {
                var entity = new QueryHistory
                {
                    UserId = user.Id,
                    CreationTime = DateTime.Now,
                    MainUsername = "aaa",
                    IsReliableSource = false,
                };
                c.QueryHistories.Add(entity);
                c.SaveChanges();
                return entity.Id;
            });

            // act
            var task = _queryHistoryAppService.GetQuerySummary(new GetQuerySummaryInput
            {
                QueryHistoryId = id,
            });

            // assert
            await task.ShouldThrow<UserFriendlyException>()
                .WithMessage("This query history does not have summary");
        }
    }
}
