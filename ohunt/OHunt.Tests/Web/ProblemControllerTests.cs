using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Flurl.Http.Testing;
using OHunt.Tests.Dependency;
using OHunt.Web;
using OHunt.Web.Controllers.Dto;
using Xunit;
using Xunit.Abstractions;

namespace OHunt.Tests.Web
{
    public class ProblemControllerTests : OHuntTestBase
    {
        public ProblemControllerTests(
            TestWebApplicationFactory<Startup> factory,
            ITestOutputHelper outputHelper) : base(factory, outputHelper)
        {
            // to make Flurl test work correctly
            // see https://github.com/tmenier/Flurl/issues/504#issuecomment-601817280
            Factory.Server.PreserveExecutionContext = true;
        }
        [Fact]
        public async Task TestResolveLabel()
        {
            // When request label at the first time
            var httpTest = new HttpTest();
            httpTest.RespondWithJson(new
            {
                num = 2001,
            });

            using var client = Factory.CreateClient();
            var response = await client.PostAsync("/api/ohunt/problems/resolve-label",
                new StringContent("{\"onlineJudge\": \"uva\", \"list\": [1]}",
                    Encoding.Default,
                    MediaTypeNames.Application.Json));

            // Then we should get the correct label
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var output = await ResponseJson<ResolveLabelOutput>(response);
            output.Result.Should().HaveCount(1);
            output.Result.Single().Should().Be(
                new KeyValuePair<string, string?>("1", "2001"));

            // And a request should be made by crawler
            httpTest.ShouldHaveMadeACall();
            httpTest.CallLog.Should().HaveCount(1);
            httpTest.Dispose();

            // When we request it again
            httpTest = new HttpTest();
            using var client2 = Factory.CreateClient();
            response = await client2.PostAsync("/api/ohunt/problems/resolve-label",
                new StringContent("{\"onlineJudge\": \"uva\", \"list\": [1]}",
                    Encoding.Default,
                    MediaTypeNames.Application.Json));

            // Then we should get the same result
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var output2 = await ResponseJson<ResolveLabelOutput>(response);
            output2.Result.Should().HaveCount(1);
            output2.Result.Single().Should().Be(
                new KeyValuePair<string, string?>("1", "2001"));

            // And there is not crawler request made
            httpTest.ShouldNotHaveMadeACall();
            httpTest.Dispose();
        }
    }
}
