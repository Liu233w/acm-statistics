// <copyright file="SessionAppService_Tests.cs" company="西北工业大学ACM开发组">
// Copyright (c) 西北工业大学ACM开发组. All rights reserved.
// </copyright>

namespace AcmStatisticsAbp.Tests.Sessions
{
    using System.Threading.Tasks;
    using AcmStatisticsAbp.Sessions;
    using Shouldly;
    using Xunit;

    public class SessionAppService_Tests : AcmStatisticsAbpTestBase
    {
        private readonly ISessionAppService sessionAppService;

        public SessionAppService_Tests()
        {
            this.sessionAppService = this.Resolve<ISessionAppService>();
        }

        [MultiTenantFact]
        public async Task Should_Get_Current_User_When_Logged_In_As_Host()
        {
            // Arrange
            this.LoginAsHostAdmin();

            // Act
            var output = await this.sessionAppService.GetCurrentLoginInformations();

            // Assert
            var currentUser = await this.GetCurrentUserAsync();
            output.User.ShouldNotBe(null);
            output.User.Name.ShouldBe(currentUser.Name);
            output.User.Surname.ShouldBe(currentUser.Surname);

            output.Tenant.ShouldBe(null);
        }

        [Fact]
        public async Task Should_Get_Current_User_And_Tenant_When_Logged_In_As_Tenant()
        {
            // Act
            var output = await this.sessionAppService.GetCurrentLoginInformations();

            // Assert
            var currentUser = await this.GetCurrentUserAsync();
            var currentTenant = await this.GetCurrentTenantAsync();

            output.User.ShouldNotBe(null);
            output.User.Name.ShouldBe(currentUser.Name);

            output.Tenant.ShouldNotBe(null);
            output.Tenant.Name.ShouldBe(currentTenant.Name);
        }
    }
}
