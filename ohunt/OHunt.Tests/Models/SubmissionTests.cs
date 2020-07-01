using System;
using FluentAssertions;
using OHunt.Web.Models;
using Xunit;

namespace OHunt.Tests.Models
{
    public class SubmissionTests
    {
        [Fact]
        public void GetValues_ShouldFormatCorrectly()
        {
            var submission = new Submission
            {
                SubmissionId = 9999999,
                OnlineJudgeId = OnlineJudge.ZOJ,
                ProblemLabel = "1001",
                Status = RunResult.Accepted,
                Time = DateTime.Parse("2020-04-01T00:00:00-05:00"),
                UserName = "vjudge",
            };

            submission.GetValues().Should().BeEquivalentTo(new string[]
            {
                "9999999",
                "1",
                "1001",
                "1",
                "2020-04-01 05:00:00",
                "vjudge",
            });
        }
    }
}
