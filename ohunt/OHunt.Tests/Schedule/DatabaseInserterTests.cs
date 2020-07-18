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
            var bufferBlock = new BufferBlock<Submission>();
            var task = _inserter.WorkAsync(bufferBlock, CancellationToken.None);
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
            await bufferBlock.SendAsync(submission);
            bufferBlock.Complete();
            await task;

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
            var bufferBlock = new BufferBlock<Submission>();
            var cancel = new CancellationTokenSource();
            var task = _inserter.WorkAsync(bufferBlock, cancel.Token);

            // act
            for (int i = 0; i < 15; i++)
            {
                await bufferBlock.SendAsync(new Submission
                {
                    Status = RunResult.Accepted,
                    Time = new DateTime(2020, 4, 1),
                    ProblemLabel = "1001",
                    UserName = "user1",
                    OnlineJudgeId = OnlineJudge.ZOJ,
                    SubmissionId = 1 + i,
                });
            }

            // inserter does not insert in single core devices (CI)
            // use the code to do so
            await Task.Delay(TimeSpan.FromSeconds(1));

            // assert
            WithDb(ctx =>
            {
                ctx.Submission.Count().Should().Be(10);
                ctx.Submission.Select(e => e.SubmissionId)
                    .Should()
                    .BeEquivalentTo(Enumerable.Range(1, 10).Select(i => (long) i));
            });

            // clean up
            cancel.Cancel();
            await task.ShouldResult().ThrowAsync<TaskCanceledException>();
        }
    }
}
