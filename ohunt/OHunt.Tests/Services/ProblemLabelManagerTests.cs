using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Flurl.Http.Testing;
using Microsoft.Extensions.DependencyInjection;
using OHunt.Tests.Dependency;
using OHunt.Web;
using OHunt.Web.Models;
using OHunt.Web.Services;
using Xunit;
using Xunit.Abstractions;

namespace OHunt.Tests.Services
{
    public class ProblemLabelManagerTests : OHuntTestBase
    {
        [Fact]
        public async Task WhenTheRecordIsInDatabase_ItShouldReadFromDatabase()
        {
            // arrange
            using var httpTest = new HttpTest();
            WithDb(context =>
            {
                context.ProblemLabelMappings.Add(new ProblemLabelMapping
                {
                    ProblemId = 1,
                    OnlineJudgeId = MappingOnlineJudge.UVA,
                    ProblemLabel = "1001",
                });
                context.SaveChanges();
            });

            using var scope = Factory.Services.CreateScope();
            var manager = scope.ServiceProvider.GetService<ProblemLabelManager>();

            // act
            var res = await manager.ResolveProblemLabel(MappingOnlineJudge.UVA, 1);

            // assert
            res.Should().Be("1001");
            httpTest.ShouldNotHaveMadeACall();
        }

        [Fact]
        public async Task WhenTheRecordIsNotInDatabase_ItShouldRequestAndSaveToDatabase()
        {
            // arrange
            using var httpTest = new HttpTest();
            httpTest.RespondWithJson(new
            {
                num = 2001,
            });

            using var scope = Factory.Services.CreateScope();
            var manager = scope.ServiceProvider.GetService<ProblemLabelManager>();

            // act
            var res = await manager.ResolveProblemLabel(MappingOnlineJudge.UVA, 2);

            // assert
            res.Should().Be("2001");
            httpTest.ShouldHaveCalled("https://uhunt.onlinejudge.org/api/p/id/2");
            httpTest.CallLog.Should().HaveCount(1);
            WithDb(context =>
            {
                context.ProblemLabelMappings.Should().HaveCount(1);
                context.ProblemLabelMappings.Single().Should().BeEquivalentTo(new ProblemLabelMapping
                {
                    ProblemId = 2,
                    ProblemLabel = "2001",
                    OnlineJudgeId = MappingOnlineJudge.UVA,
                });
            });
        }

        public ProblemLabelManagerTests(
            TestWebApplicationFactory<Startup> factory,
            ITestOutputHelper outputHelper) : base(factory, outputHelper)
        {
        }
    }
}
