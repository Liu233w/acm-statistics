using System;
using System.Linq;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using FluentAssertions;
using OHunt.Web.Crawlers;
using OHunt.Web.Dataflow;
using OHunt.Web.Models;
using Xunit;

namespace OHunt.Tests.Dataflow
{
    public class CrawlerPropagatorTests
    {
        private readonly CrawlerPropagator _propagator;

        private readonly BufferBlock<Submission> _submissionOutput;
        private readonly BufferBlock<CrawlerError> _errorOutput;

        public CrawlerPropagatorTests()
        {
            _submissionOutput = new BufferBlock<Submission>();
            _errorOutput = new BufferBlock<CrawlerError>();

            _propagator = new CrawlerPropagator(_submissionOutput, _errorOutput);
        }

        [Fact]
        public async Task It_ShouldWork()
        {
            // Given a propagator

            // When receiving data
            await _propagator.SendAsync(new CrawlerMessage
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
                CrawlerError = new CrawlerError
                {
                    Crawler = "zoj",
                    Data = null,
                    Message = "An error",
                    Time = new DateTime(2020, 4, 1, 1, 0, 0),
                },
            });

            // Then they are cached
            await Utils.WaitSecond();
            _submissionOutput.Count.Should().Be(0);
            _errorOutput.Count.Should().Be(0);

            // When a checkpoint is sent
            await _propagator.SendAsync(new CrawlerMessage { Checkpoint = true });

            // Then data are dispatched
            await Utils.WaitSecond();
            _submissionOutput.Count.Should().Be(1);
            _errorOutput.Count.Should().Be(1);
        }

        [Fact]
        public async Task WhenReceivingRollback_ItShouldRollbackToLastCheckpoint()
        {
            // arrange
            await _propagator.SendAsync(new CrawlerMessage
            {
                Submission = new Submission
                {
                    SubmissionId = 1,
                },
                Checkpoint = true,
            });

            await _propagator.SendAsync(new CrawlerMessage
            {
                Submission = new Submission
                {
                    SubmissionId = 2,
                },
            });

            // act
            await _propagator.SendAsync(new CrawlerMessage
            {
                Submission = new Submission
                {
                    SubmissionId = 3,
                },
                Rollback = true,
            });

            await _propagator.SendAsync(new CrawlerMessage
            {
                Submission = new Submission
            {
                    SubmissionId = 4,
                },
                Checkpoint = true,
            });

            _propagator.Complete();
            await _propagator.Completion;

            // assert
            _submissionOutput.TryReceiveAll(out var res)
                .Should().BeTrue();
            res.Select(item => item.SubmissionId)
                .Should()
                .Equal(1, 4);
        }

        [Fact]
        public async Task WhenPropagatorCompletes_OutputsShouldBeCompleted()
        {
            // arrange
            await _propagator.SendAsync(new CrawlerMessage
            {
                Submission = new Submission
                {
                    SubmissionId = 1,
                },
                Checkpoint = true,
            });

            // act
            _propagator.Complete();

            // assert
            await _errorOutput.Completion;
            _submissionOutput.TryReceive(out var item)
                .Should().BeTrue();
            item.SubmissionId.Should().Be(1);
            await _submissionOutput.Completion;
        }

        [Fact]
        public async Task WhenPropagatorFault_OutputShouldFault()
        {
            // act
            _propagator.Fault(new Exception("An error"));

            // assert
            (await _errorOutput.Completion
                    .ShouldResult().ThrowAsync<AggregateException>())
                .WithInnerException<Exception>()
                .WithMessage("An error");
            (await _submissionOutput.Completion
                    .ShouldResult().ThrowAsync<AggregateException>())
                .WithInnerException<Exception>()
                .WithMessage("An error");
        }
    }
}
