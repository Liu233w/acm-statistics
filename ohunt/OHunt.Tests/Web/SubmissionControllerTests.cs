using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json.Linq;
using OHunt.Tests.Dependency;
using OHunt.Web;
using OHunt.Web.Models;
using Xunit;
using Xunit.Abstractions;

namespace OHunt.Tests.Web
{
    public class SubmissionControllerTests : OHuntTestBase
    {
        public SubmissionControllerTests(
            TestWebApplicationFactory<Startup> factory,
            ITestOutputHelper outputHelper) : base(factory, outputHelper)
        {
        }

        [Fact]
        public async Task WhenRequestingItems_ItShouldResponse()
        {
            // arrange
            WithDb(context =>
            {
                context.Submission.AddRange(Enumerable.Range(1, 10)
                    .Select(i => new Submission
                    {
                        Status = RunResult.Accepted,
                        Time = new DateTime(2020, 4, 1, 0, 0, 0),
                        ProblemLabel = "1001",
                        SubmissionId = i,
                        UserName = "user1",
                        OnlineJudgeId = OnlineJudge.ZOJ,
                    }));
                context.SaveChanges();
            });

            // act
            var client = Factory.CreateClient();
            var res = await client.GetAsync(
                "/api/ohunt/submissions?oj=zoj&$filter=submissionId eq 1");

            // assert
            res.StatusCode.Should().Be(StatusCodes.Status200OK);
            JObject.Parse(await res.Content.ReadAsStringAsync())
                ["value"]
                ?.ToObject<Submission[]>()
                .Should()
                .BeEquivalentTo(new Submission
                {
                    Status = RunResult.Accepted,
                    Time = new DateTime(2020, 4, 1, 0, 0, 0),
                    ProblemLabel = "1001",
                    SubmissionId = 1,
                    UserName = "user1",
                    OnlineJudgeId = OnlineJudge.ZOJ,
                });
        }

        [Fact]
        public async Task WhenOjIsNotSpecified_ItShouldReturnError()
        {
            // act
            var client = Factory.CreateClient();
            var res = await client.GetAsync(
                "/api/ohunt/submissions?$filter=submissionId eq 1");

            // assert
            res.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
            var json = JObject.Parse(await res.Content.ReadAsStringAsync())!;
            json["error"]?.ToObject<bool>().Should().BeTrue();
            json["message"]?.ToObject<string>().Should().Be("Unrecognisable OJ name");
            json["detail"]?.ToObject<string>().Should()
                .Be("Please use url like /api/ohunt/submissions?oj=zoj to request");
            json["supportedOj"]?.ToObject<string[]>().Should()
                .BeEquivalentTo(Enum.GetNames(typeof(OnlineJudge)));
        }

        [Fact]
        public async Task WhenTooManyItemsRequested_ItShouldReturnAtMost500Items()
        {
            // arrange
            WithDb(context =>
            {
                context.Submission.AddRange(Enumerable.Range(1, 1000)
                    .Select(i => new Submission
                    {
                        Status = RunResult.Accepted,
                        Time = new DateTime(2020, 4, 1, 0, 0, 0),
                        ProblemLabel = "1001",
                        SubmissionId = i,
                        UserName = "user1",
                        OnlineJudgeId = OnlineJudge.ZOJ,
                    }));
                context.SaveChanges();
            });

            // act
            var client = Factory.CreateClient();
            var res = await client.GetAsync(
                "/api/ohunt/submissions?oj=zoj");

            // assert
            res.StatusCode.Should().Be(StatusCodes.Status200OK);
            var json = JObject.Parse(await res.Content.ReadAsStringAsync());
            json["value"]?.ToObject<Submission[]>().Should().HaveCount(500);
            json["@odata.nextLink"]?.ToObject<string>()
                .Should().EndWith("/api/ohunt/submissions?oj=zoj&$skip=500");
        }
    }
}
