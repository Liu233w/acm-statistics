using System;
using System.Linq;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using OHunt.Tests.Dependency;
using OHunt.Web;
using OHunt.Web.Dataflow;
using OHunt.Web.Models;
using OHunt.Web.Options;
using Xunit;
using Xunit.Abstractions;

namespace OHunt.Tests.Dataflow
{
    public class DatabaseInserterTests : OHuntTestBase
    {
        public DatabaseInserterTests(TestWebApplicationFactory<Startup> factory, ITestOutputHelper outputHelper) : base(
            factory, outputHelper)
        {
            _inserter = Factory.Services
                .GetService<DatabaseInserterFactory>()
                .CreateInstance<Submission>();
        }

        private readonly DatabaseInserter<Submission> _inserter;

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                services.Configure<DatabaseInserterOptions>(opts => { opts.DefaultBufferSize = 10; });
            });
        }

        [Fact]
        public async Task WhenPipelineComplete_ItShouldInsertRecords()
        {
            // arrange
            var submission = new Submission
            {
                Status = RunResult.Accepted,
                Time = new DateTime(2020, 4, 1),
                ProblemLabel = "1001",
                UserName = "user1",
                OnlineJudgeId = OnlineJudge.ZOJ,
                SubmissionId = 1000000,
            };

            // act
            await _inserter.SendAsync(DatabaseInserterMessage<Submission>
                .OfEntity(submission));
            _inserter.Complete();
            await _inserter.Completion;

            // assert
            WithDb(ctx =>
            {
                ctx.Submission.Count().Should().Be(1);
                ctx.Submission.Single().Should().BeEquivalentTo(submission);
            });
        }

        [Fact]
        public async Task WhenEnoughRecordsSent_ItShouldInsertRecords()
        {
            // act
            for (int i = 0; i < 15; i++)
            {
                await _inserter.SendAsync(DatabaseInserterMessage<Submission>
                    .OfEntity(new Submission
                        {
                            Status = RunResult.Accepted,
                            Time = new DateTime(2020, 4, 1),
                            ProblemLabel = "1001",
                            UserName = "user1",
                            OnlineJudgeId = OnlineJudge.ZOJ,
                            SubmissionId = 1 + i,
                        }
                    ));
            }

            await Utils.WaitSecond();

            // assert
            WithDb(ctx =>
            {
                ctx.Submission.Count().Should().Be(10);
                ctx.Submission.Select(e => e.SubmissionId)
                    .Should()
                    .Equal(Enumerable.Range(1, 10).Select(i => (long) i));
            });
        }

        [Fact]
        public async Task WhenForceInsertIsTrue_ItShouldInsertRecords()
        {
            // arrange
            var submission = new Submission
            {
                Status = RunResult.Accepted,
                Time = new DateTime(2020, 4, 1),
                ProblemLabel = "1001",
                UserName = "user1",
                OnlineJudgeId = OnlineJudge.ZOJ,
                SubmissionId = 1000000,
            };

            // act
            await _inserter.SendAsync(DatabaseInserterMessage<Submission>
                .OfEntity(submission, true));

            // wait for ActionBlock executing
            await Utils.WaitSecond();

            // assert
            WithDb(ctx => { ctx.Submission.Count().Should().Be(1); });
        }

        [Fact]
        public async Task WhenRequestingComplete_ItShouldInsertAllDataBeforeCompletion()
        {
            // arrange
            var submission = new Submission
            {
                Status = RunResult.Accepted,
                Time = new DateTime(2020, 4, 1),
                ProblemLabel = "1001",
                UserName = "user1",
                OnlineJudgeId = OnlineJudge.ZOJ,
                SubmissionId = 1000000,
            };
            await _inserter.SendAsync(DatabaseInserterMessage<Submission>
                .OfEntity(submission));

            // act
            _inserter.Complete();

            // assert
            await _inserter.Completion;
            WithDb(context => { context.Submission.Should().HaveCount(1); });
        }
    }
}
