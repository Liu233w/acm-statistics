using OHunt.Web;
using Xunit;

namespace OHunt.Tests.Dependency
{
    public abstract class OHuntTestBase
        : IClassFixture<TestWebApplicationFactory<Startup>>
    {
        protected readonly TestWebApplicationFactory<Startup> Factory;

        protected OHuntTestBase(TestWebApplicationFactory<Startup> factory)
        {
            Factory = factory;
        }
    }
}
