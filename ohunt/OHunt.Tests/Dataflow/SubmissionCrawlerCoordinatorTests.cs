using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using OHunt.Tests.Dependency;
using OHunt.Web;
using OHunt.Web.Crawlers;
using OHunt.Web.Dataflow;
using OHunt.Web.Models;
using Xbehave;
using Xunit;
using Xunit.Abstractions;

namespace OHunt.Tests.Dataflow
{
    public class SubmissionCrawlerCoordinatorTests : OHuntTestBase
    {
        private readonly SubmissionCrawlerCoordinator _coordinator;
        private readonly CrawlerMock _crawlerMock;

        public SubmissionCrawlerCoordinatorTests(
            TestWebApplicationFactory<Startup> factory,
            ITestOutputHelper outputHelper) : base(factory, outputHelper)
        {
            _coordinator = Factory.Services.GetService<SubmissionCrawlerCoordinator>();
            _crawlerMock = new CrawlerMock();
        }

        [Scenario]
        public void It_ShouldWork()
        {
            "Given a coordinator".x(() => { });

            "After it is initialized"
                .x(() => _coordinator.Initialize(new[] { _crawlerMock }));

            "Crawler does not immediately work"
                .x(() => _crawlerMock.CalledCount.Should().Be(0));

            $"After calling {nameof(_coordinator.StartAllCrawlers)}"
                .x(() => _coordinator.StartAllCrawlers());

            $"Crawler's {nameof(_crawlerMock.WorkAsync)} is called"
                .x(() => _crawlerMock.CalledCount.Should().Be(1));

            "At first, there is nothing in the database"
                .x(() => WithDb(context =>
                {
                    context.Submission.Should().BeEmpty();
                    context.CrawlerErrors.Should().BeEmpty();
                }));

            "So lastSubmissionId should be null"
                .x(() => _crawlerMock.LastSubmissionId.Should().BeNull());

            "When crawler sends data"
                .x(async () =>
                {
                    await _crawlerMock.Pipeline.SendAsync(new CrawlerMessage
                    {
                        Submission = new Submission
                        {
                            Status = RunResult.Accepted,
                            Time = new DateTime(2020, 4, 1, 0, 0, 0),
                            ProblemLabel = "1001",
                            SubmissionId = 42,
                            UserName = "user1",
                            OnlineJudgeId = OnlineJudge.ZOJ,
                        },
                    });
                    await _crawlerMock.Pipeline.SendAsync(new CrawlerMessage
                    {
                        CrawlerError = new CrawlerError
                        {
                            Crawler = "zoj",
                            Data = null,
                            Message = "An error",
                            Time = new DateTime(2020, 4, 1, 1, 0, 0),
                        },
                    });
                    await Task.Delay(TimeSpan.FromSeconds(5));
                });

            "They are not saved to database immediately"
                .x(() => WithDb(context =>
                {
                    context.Submission.Should().BeEmpty();
                    context.CrawlerErrors.Should().BeEmpty();
                }));

            "When crawler sends a checkpoint"
                .x(async () =>
                {
                    await _crawlerMock.Pipeline.SendAsync(new CrawlerMessage
                    {
                        IsCheckPoint = true,
                    });
                    await Task.Delay(TimeSpan.FromSeconds(5));
                });

            "Data are not saved to database immediately"
                .x(() => WithDb(context =>
                {
                    context.Submission.Should().BeEmpty();
                    context.CrawlerErrors.Should().BeEmpty();
                }));

            "When crawler finished"
                .x(async () =>
                {
                    _crawlerMock.TaskSource.SetResult(1);
                    // wait longer to insert database
                    await Task.Delay(TimeSpan.FromSeconds(5));
                });

            "Data are saved to database"
                .x(() => WithDb(context =>
                {
                    context.Submission.Should().HaveCount(1);
                    context.Submission.Should().AllBeEquivalentTo(new Submission
                    {
                        Status = RunResult.Accepted,
                        Time = new DateTime(2020, 4, 1, 0, 0, 0),
                        ProblemLabel = "1001",
                        SubmissionId = 42,
                        UserName = "user1",
                        OnlineJudgeId = OnlineJudge.ZOJ,
                    });

                    context.CrawlerErrors.Should().HaveCount(1);
                    context.CrawlerErrors.Single().Invoking(it =>
                    {
                        it.Crawler.Should().Be("zoj");
                        it.Data.Should().BeNull();
                        it.Message.Should().Be("An error");
                        it.Time.Should().Be(new DateTime(2020, 4, 1, 1, 0, 0));
                        it.Id.Should().NotBe(0);
                    });
                }));

            "Finally, coordinator can be cancelled"
                .x(() => _coordinator.Cancel());
        }

        [Fact]
        public async Task WhenSendingOver1000Entities_TheyShouldBeInsertedToDatabase()
        {
            // arrange
            _coordinator.Initialize(new[] { _crawlerMock });
            _coordinator.StartAllCrawlers();

            // act
            for (int i = 0; i < 11; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    await _crawlerMock.Pipeline.SendAsync(new CrawlerMessage
                    {
                        CrawlerError = new CrawlerError
                        {
                            Crawler = "zoj",
                            Data = null,
                            Message = "message",
                            Time = new DateTime(2020, 4, 1, 0, 0, 0),
                        }
                    });
                }

                await _crawlerMock.Pipeline.SendAsync(new CrawlerMessage
                {
                    IsCheckPoint = true,
                });
            }

            await Task.Delay(TimeSpan.FromSeconds(1));

            // assert
            WithDb(context => { context.CrawlerErrors.Should().HaveCount(100); });
        }

        private class CrawlerMock : ISubmissionCrawler
        {
            public TaskCompletionSource<int> TaskSource { get; set; }

            public long? LastSubmissionId { get; set; }
            public ITargetBlock<CrawlerMessage> Pipeline { get; set; }
            public CancellationToken CancellationToken { get; set; }

            public int CalledCount { get; private set; } = 0;

            public OnlineJudge OnlineJudge => OnlineJudge.ZOJ;

            public Task WorkAsync(
                long? lastSubmissionId,
                ITargetBlock<CrawlerMessage> pipeline,
                CancellationToken cancellationToken)
            {
                LastSubmissionId = lastSubmissionId;
                Pipeline = pipeline;
                CancellationToken = cancellationToken;

                ++CalledCount;

                TaskSource = new TaskCompletionSource<int>();
                return TaskSource.Task;
            }
        }
    }
}
