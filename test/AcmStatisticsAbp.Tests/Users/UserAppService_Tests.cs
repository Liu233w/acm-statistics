// <copyright file="UserAppService_Tests.cs" company="西北工业大学ACM开发组">
// Copyright (c) 西北工业大学ACM开发组. All rights reserved.
// </copyright>

namespace AcmStatisticsAbp.Tests.Users
{
    using System.Threading.Tasks;
    using Abp.Application.Services.Dto;
    using AcmStatisticsAbp.Users;
    using AcmStatisticsAbp.Users.Dto;
    using Microsoft.EntityFrameworkCore;
    using Shouldly;
    using Xunit;

    public class UserAppService_Tests : AcmStatisticsAbpTestBase
    {
        private readonly IUserAppService userAppService;

        public UserAppService_Tests()
        {
            this.userAppService = this.Resolve<IUserAppService>();
        }

        [Fact]
        public async Task GetUsers_Test()
        {
            // Act
            var output = await this.userAppService.GetAll(new PagedResultRequestDto { MaxResultCount = 20, SkipCount = 0 });

            // Assert
            output.Items.Count.ShouldBeGreaterThan(0);
        }

        [Fact]
        public async Task CreateUser_Test()
        {
            // Act
            await this.userAppService.Create(
                new CreateUserDto
                {
                    EmailAddress = "john@volosoft.com",
                    IsActive = true,
                    Name = "John",
                    Surname = "Nash",
                    Password = "123qwe",
                    UserName = "john.nash",
                });

            await this.UsingDbContextAsync(async context =>
            {
                var johnNashUser = await context.Users.FirstOrDefaultAsync(u => u.UserName == "john.nash");
                johnNashUser.ShouldNotBeNull();
            });
        }
    }
}
