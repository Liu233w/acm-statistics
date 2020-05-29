using System.Threading.Tasks;
using AcmStatisticsBackend.Sessions;
using FluentAssertions;
using Xunit;

namespace AcmStatisticsBackend.Tests.Sessions
{
    public class SessionAppService_Tests : AcmStatisticsBackendTestBase
    {
        private readonly ISessionAppService _sessionAppService;

        public SessionAppService_Tests()
        {
            _sessionAppService = Resolve<ISessionAppService>();
        }

        [Fact]
        public async Task Should_Get_Current_User_And_Tenant_When_Logged_In_As_Tenant()
        {
            // Act
            var output = await _sessionAppService.GetCurrentLoginInformations();

            // Assert
            var currentUser = await GetCurrentUserAsync();
            var currentTenant = await GetCurrentTenantAsync();

            output.User.Should().NotBe(null);
            output.User.Name.Should().Be(currentUser.Name);

            output.Tenant.Should().NotBe(null);
            output.Tenant.Name.Should().Be(currentTenant.Name);
        }
    }
}
