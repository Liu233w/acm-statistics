using System.Threading.Tasks;
using OHunt.Tests.Dependency;
using OHunt.Web;
using Snapshooter.Xunit;
using Xunit;
using Xunit.Abstractions;

namespace OHunt.Tests.Web
{
    public class SwaggerTests : OHuntTestBase
    {
        [Fact]
        public async Task It_ShouldOutputDocument()
        {
            // arrange
            var httpClient = Factory.CreateClient();

            // act
            var result = await httpClient.GetAsync("/api/ohunt/v1/swagger.json");

            // assert
            result.Content.ReadAsStringAsync().Result.MatchSnapshot();
        }

        public SwaggerTests(TestWebApplicationFactory<Startup> factory, ITestOutputHelper outputHelper) : base(factory,
            outputHelper)
        {
        }
    }
}
