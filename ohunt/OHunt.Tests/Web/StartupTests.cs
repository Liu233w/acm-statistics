using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
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
            result.StatusCode.Should().Be(StatusCodes.Status404NotFound);
            (await result.Content.ReadAsStringAsync())
                .Should().Be("404 Not Found");
        }

        public StartupTests(TestWebApplicationFactory<Startup> factory, ITestOutputHelper outputHelper) : base(factory, outputHelper)
        {
        }
    }
}
