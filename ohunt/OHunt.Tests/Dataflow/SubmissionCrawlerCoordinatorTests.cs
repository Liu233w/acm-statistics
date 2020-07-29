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
    public class SubmissionCrawlerCoordinatorTests : OHuntTestBase, IDisposable
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

            "When it is initialized"
                .x(() => _coordinator.Initialize(new[] { _crawlerMock }));

            "But crawler does not immediately work"
                .x(() => _crawlerMock.CalledCount.Should().Be(0));

            $"When calling {nameof(_coordinator.StartAllCrawlers)}"
                .x(() => _coordinator.StartAllCrawlers());

            $"Then crawler's {nameof(_crawlerMock.WorkAsync)} is called"
                .x(() => _crawlerMock.CalledCount.Should().Be(1));

            "And there is nothing in the database"
                .x(() => WithDb(context =>
                {
                    context.Submission.Should().BeEmpty();
                    context.CrawlerErrors.Should().BeEmpty();
                }));

            "And lastSubmissionId sent to crawler should be null"
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
                    await Utils.WaitSecond();
                });

            "But data are not saved to database immediately"
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
                        Checkpoint = true,
                    });
                    await Utils.WaitSecond();
                });

            "But data are not saved to database immediately"
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
                    await Utils.WaitSecond();
                });

            "Then data are saved to database"
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

            "When cancelling a coordinator, it should not throw"
                .x(() => _coordinator.Cancel());
        }

        [Fact]
        public async Task WhenSendingOver1000Entities_TheyShouldBeInsertedToDatabase()
        {
            // arrange
            _coordinator.Initialize(new[] { _crawlerMock });
            await _coordinator.StartAllCrawlers();

            // act
            for (int i = 0; i < 11; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    await SendToPipeline(new CrawlerMessage
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

                await SendToPipeline(new CrawlerMessage
                {
                    Checkpoint = true,
                });
            }

            await Utils.WaitSecond();

            // assert
            WithDb(context => { context.CrawlerErrors.Should().HaveCount(100); });
        }

        [Fact]
        public void WhenInitializeTwice_ItShouldThrow()
        {
            _coordinator.Initialize(new ISubmissionCrawler[0]);
            FluentActions.Invoking(() => _coordinator.Initialize(new ISubmissionCrawler[0]))
                .Should().ThrowExactly<InvalidOperationException>();
        }

        [Fact]
        public void WhenNotInitialized_ItShouldThrow()
        {
            FluentActions.Invoking(() => _coordinator.StartAllCrawlers())
                .Should().ThrowExactly<InvalidOperationException>();
            FluentActions.Invoking(() => _coordinator.Cancel())
                .Should().ThrowExactly<InvalidOperationException>();
        }

        [Scenario]
        public void WhenCrawlerThrows()
        {
            "Given an initialized coordinator"
                .x(() =>
                {
                    _coordinator.Initialize(new[] { _crawlerMock });
                    _coordinator.StartAllCrawlers();
                });

            "And some data"
                .x(async () =>
                {
                    _crawlerMock.CalledCount.Should().Be(1);

                    await SendToPipeline(new CrawlerMessage
                    {
                        Submission = new Submission { SubmissionId = 1 },
                        Checkpoint = true,
                    });
                    await SendToPipeline(new CrawlerMessage
                    {
                        Submission = new Submission { SubmissionId = 2 },
                    });
                });

            "When crawler throws"
                .x(() => _crawlerMock.TaskSource.SetException(new Exception("Crawler throws")));

            "Then coordinator should complete"
                .x(async () =>
                {
                    // cannot test it
                    await Utils.WaitSecond();
                });

            "And data before checkpoint should be saved"
                .x(() => WithDb(context =>
                {
                    context.Submission.Should().HaveCount(1);
                    context.Submission.Single().Should()
                        .Match(it => it.As<Submission>().SubmissionId == 1);
                }));

            "When start coordinator again"
                .x(() => _coordinator.StartAllCrawlers());

            "And send some data"
                .x(() =>
                {
                    _crawlerMock.CalledCount.Should().Be(2);
                    return SendToPipeline(new CrawlerMessage
                    {
                        Submission = new Submission { SubmissionId = 3 },
                        Checkpoint = true,
                    });
                });

            "And complete crawler"
                .x(() => _crawlerMock.TaskSource.SetResult(1));

            "Then data should be saved"
                .x(async () =>
                {
                    await Utils.WaitSecond();
                    WithDb(context =>
                    {
                        context.Submission.Should().HaveCount(2);
                        context.Submission.Select(it => it.SubmissionId)
                            .Should().Equal(1, 3);
                    });
                });
        }

        [Fact]
        public async Task AfterFinished_WhenStartAgain_ItShouldResume()
        {
            // arrange
            _coordinator.Initialize(new[] { _crawlerMock });
            await _coordinator.StartAllCrawlers();
            await SendToPipeline(new CrawlerMessage
            {
                Submission = new Submission { SubmissionId = 1 },
                Checkpoint = true,
            });
            _crawlerMock.TaskSource.SetResult(1);
            await Utils.WaitSecond();

            // act
            await _coordinator.StartAllCrawlers();
            _crawlerMock.CalledCount.Should().Be(2);
            await SendToPipeline(new CrawlerMessage
            {
                Submission = new Submission { SubmissionId = 2 },
                Checkpoint = true,
            });
            _crawlerMock.TaskSource.SetResult(1);
            await Utils.WaitSecond();

            // assert
            WithDb(context =>
            {
                context.Submission.Should().HaveCount(2);
                context.Submission.Select(it => it.SubmissionId)
                    .Should().Equal(1, 2);
            });
        }

        [Scenario]
        public void WhenCancelled(Task cancelTask)
        {
            "Given an initialized coordinator"
                .x(() =>
                {
                    _coordinator.Initialize(new[] { _crawlerMock });
                    _coordinator.StartAllCrawlers();
                });

            "And some data"
                .x(async () =>
                {
                    await SendToPipeline(new CrawlerMessage
                    {
                        Submission = new Submission { SubmissionId = 1 },
                        Checkpoint = true,
                    });
                    await SendToPipeline(new CrawlerMessage
                    {
                        Submission = new Submission { SubmissionId = 2 },
                    });
                });

            "When calling coordinator.cancel"
                .x(() => { cancelTask = _coordinator.Cancel(); });

            "Then crawler should receive cancel"
                .x(() => _crawlerMock.CancellationToken.IsCancellationRequested.Should().BeTrue());

            "And cancelling task is still not completed"
                .x(() => cancelTask.IsCompleted.Should().BeFalse());

            "And crawler can still sending data until exit"
                .x(async () =>
                {
                    await SendToPipeline(new CrawlerMessage
                    {
                        Submission = new Submission { SubmissionId = 3 },
                        Checkpoint = true,
                    });
                    await SendToPipeline(new CrawlerMessage
                    {
                        Submission = new Submission { SubmissionId = 4 },
                    });
                });

            "When crawler is exited"
                .x(() => _crawlerMock.TaskSource.SetCanceled());

            "Then cancel task is completed"
                .x(() => cancelTask);

            "And data before checkpoint is saved to database without needing to wait"
                .x(() => WithDb(context =>
                {
                    context.Submission.Should().HaveCount(3);
                    context.Submission.Select(it => it.SubmissionId)
                        .Should().Equal(1, 2, 3);
                }));

            "When start crawlers without initializing, it should throw"
                .x(() => _coordinator.StartAllCrawlers()
                    .ShouldResult().ThrowAsync<InvalidOperationException>());

            "When restarting crawlers"
                .x(async () =>
                {
                    _coordinator.Initialize(new[] { _crawlerMock });
                    await _coordinator.StartAllCrawlers();
                    await Utils.WaitSecond();
                    _crawlerMock.CalledCount.Should().Be(2);
                });

            "And send some data and finish the crawler"
                .x(async () =>
                {
                    await SendToPipeline(new CrawlerMessage
                    {
                        Checkpoint = true,
                        Submission = new Submission { SubmissionId = 5 },
                    });
                    _crawlerMock.TaskSource.SetResult(1);
                    await Utils.WaitSecond();
                });

            "Then data after checkpoint of previous run are discarded"
                .x(() => WithDb(context =>
                {
                    context.Submission.Should().HaveCount(4);
                    context.Submission.Select(it => it.SubmissionId)
                        .Should().Equal(1, 2, 3, 5);
                }));
        }

        [Fact]
        public async Task WhenCancellingANotRunningCoordinator_ItShouldDoNothing()
        {
            // arrange
            _coordinator.Initialize(new[] { _crawlerMock });

            // act
            await _coordinator.Cancel();

            // assert
            _coordinator.Initialize(new[] { _crawlerMock });
            await _coordinator.StartAllCrawlers();

            _crawlerMock.CalledCount.Should().Be(1);
            await SendToPipeline(new CrawlerMessage
            {
                Submission = new Submission { SubmissionId = 1 },
                Checkpoint = true,
            });

            _crawlerMock.TaskSource.SetResult(1);

            await Utils.WaitSecond();

            WithDb(context =>
            {
                context.Submission.Should().HaveCount(1);
                context.Submission.Select(it => it.SubmissionId)
                    .Should().Equal(1);
            });
        }

        [Fact]
        public async Task WhenCrawlerCompleteSuccessfully_ItShouldAutomaticallySendACheckpoint()
        {
            // arrange
            _coordinator.Initialize(new[] { _crawlerMock });
            await _coordinator.StartAllCrawlers();
            await SendToPipeline(new CrawlerMessage
            {
                Submission = new Submission { SubmissionId = 1 },
            });

            // act
            _crawlerMock.TaskSource.SetResult(1);

            await Utils.WaitSecond();

            // assert
            WithDb(context =>
            {
                context.Submission.Should().HaveCount(1);
                context.Submission.Single().SubmissionId.Should().Be(1);
            });
        }

        [Fact]
        public async Task WhenStartingCrawlerBeforePreviousFinish_ItShouldDoNothing()
        {
            // arrange
            _coordinator.Initialize(new[] { _crawlerMock });
            await _coordinator.StartAllCrawlers();

            // act
            await Utils.WaitSecond();
            await _coordinator.StartAllCrawlers();

            // assert
            await Utils.WaitSecond();
            _crawlerMock.CalledCount.Should().Be(1);
        }

        [Fact]
        public async Task WhenCancellingCrawler_ItShouldWaitForCrawlerFinishing_AndWaitForDataInserting()
        {
            // arrange
            _coordinator.Initialize(new[] { _crawlerMock });
            await _coordinator.StartAllCrawlers();
            await SendToPipeline(new CrawlerMessage
            {
                Submission = new Submission { SubmissionId = 1 },
                Checkpoint = true,
            });

            // act
            var allCancel = _coordinator.Cancel();
            _crawlerMock.CancellationToken.IsCancellationRequested.Should().BeTrue();
            _crawlerMock.TaskSource.SetCanceled();
            await allCancel;

            // assert
            WithDb(context => { context.Submission.Should().HaveCount(1); });
        }

        private Task SendToPipeline(CrawlerMessage message)
        {
            return _crawlerMock.Pipeline.SendAsync(message);
        }

        private class CrawlerMock : ISubmissionCrawler
        {
            public TaskCompletionSource<int> TaskSource { get; set; } = null!;

            public long? LastSubmissionId { get; set; }
            public ITargetBlock<CrawlerMessage> Pipeline { get; set; } = null!;
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

        public void Dispose()
        {
            _coordinator?.Dispose();
        }
    }
}
