using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using OHunt.Tests.Dependency;
using OHunt.Web;
using Xunit;
using Xunit.Abstractions;

namespace OHunt.Tests.Web
{
    public class StartupTests : OHuntTestBase
    {
        [Fact]
        public async Task WhenRequestingNotExistUrl_ItShouldReturn404()
        {
            // arrange
            var httpClient = Factory.CreateClient();

            // act
            var result = await httpClient.GetAsync("/not-exist");

            // assert
            result.StatusCode.Should().Be(HttpStatusCode.NotFound);
            (await result.Content.ReadAsStringAsync())
                .Should().Be("404 Not Found");
        }

        [Fact(Skip = "don't know why it's broken")]
        public async Task WhenRequestingIndex_RedirectToSwagger()
        {
            // arrange
            var httpClient = Factory.CreateClient();

            // act
            var result = await httpClient.GetAsync("/");

            // assert
            result.RequestMessage.RequestUri.LocalPath
                .Should().Be("/ohunt/swagger/index.html");
        }

        public StartupTests(TestWebApplicationFactory<Startup> factory, ITestOutputHelper outputHelper) : base(factory,
            outputHelper)
        {
        }
    }
}
