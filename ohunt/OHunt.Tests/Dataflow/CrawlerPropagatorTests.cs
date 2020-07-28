using System;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using FluentAssertions;
using OHunt.Web.Crawlers;
using OHunt.Web.Dataflow;
using OHunt.Web.Models;
using Xbehave;
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

        [Scenario]
        public async Task It_ShouldWork()
        {
            "Given a propagator".x(() => { });

            "When receiving data"
                .x(() => _propagator.SendAsync(new CrawlerMessage
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
                    })
                );

            "They are cached"
                .x(async () =>
                {
                    await Utils.WaitSecond();
                    _submissionOutput.Count.Should().Be(0);
                    _errorOutput.Count.Should().Be(0);
                });

            "When a checkpoint is sent"
                .x(() => _propagator.SendAsync(new CrawlerMessage { IsCheckPoint = true }));

            "Data are dispatched"
                .x(async () =>
                {
                    await Utils.WaitSecond();
                    _submissionOutput.Count.Should().Be(1);
                    _errorOutput.Count.Should().Be(1);
                });
        }

        /*
         * WhenPropagatorCompletes_OutputsShouldBeCompleted
         */
    }
}
