using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using OHunt.Tests.Dependency;
using OHunt.Web;
using OHunt.Web.Models;
using OHunt.Web.Options;
using OHunt.Web.Schedule;
using Xunit;

namespace OHunt.Tests.Schedule
{
    public class DatabaseInserterTests : OHuntTestBase
    {
        private readonly DatabaseInserter<Submission> _inserter;

        public DatabaseInserterTests(TestWebApplicationFactory<Startup> factory) : base(factory)
        {
            _inserter = Factory.Services.GetService<DatabaseInserter<Submission>>();
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                services.Configure<DatabaseInserterOptions>(opts => { opts.DefaultBufferSize = 10; });
            });
        }

        [Fact]
        public async Task WhenStreamComplete_ItShouldInsertRecords()
        {
            // arrange
            var target = _inserter.Target;
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
            await target.SendAsync(submission);
            target.Complete();

            // inserter does not insert in single core devices (CI)
            // use the code to do so
            await Task.Delay(TimeSpan.FromSeconds(1));

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
            // arrange
            var target = _inserter.Target;

            // act
            for (int i = 0; i < 15; i++)
            {
                await target.SendAsync(new Submission
                {
                    Status = RunResult.Accepted,
                    Time = new DateTime(2020, 4, 1),
                    ProblemLabel = "1001",
                    UserName = "user1",
                    OnlineJudgeId = OnlineJudge.ZOJ,
                    SubmissionId = 1 + i,
                });
            }

            // assert
            WithDb(ctx =>
            {
                ctx.Submission.Count().Should().Be(10);
                ctx.Submission.Select(e => e.SubmissionId)
                    .Should()
                    .BeEquivalentTo(Enumerable.Range(1, 10).Select(i => (long) i));
            });
        }

        [Fact]
        public async Task WhenWaitEnoughTime_ItShouldInsertRecords()
        {
            // arrange
            var target = _inserter.Target;

            // act
            for (int i = 0; i < 5; i++)
            {
                await target.SendAsync(new Submission
                {
                    Status = RunResult.Accepted,
                    Time = new DateTime(2020, 4, 1),
                    ProblemLabel = "1001",
                    UserName = "user1",
                    OnlineJudgeId = OnlineJudge.ZOJ,
                    SubmissionId = 1 + i,
                });
            }

            await Task.Delay(TimeSpan.FromSeconds(10));

            // assert
            WithDb(ctx =>
            {
                ctx.Submission.Count().Should().Be(5);
                ctx.Submission.Select(e => e.SubmissionId)
                    .Should()
                    .BeEquivalentTo(Enumerable.Range(1, 5).Select(i => (long) i));
            });
        }
    }
}
