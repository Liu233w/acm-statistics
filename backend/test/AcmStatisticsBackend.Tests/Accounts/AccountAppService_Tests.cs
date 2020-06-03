using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Authorization;
using Abp.MultiTenancy;
using Abp.UI;
using AcmStatisticsBackend.Accounts;
using AcmStatisticsBackend.Accounts.Dto;
using AcmStatisticsBackend.Authorization;
using AcmStatisticsBackend.Authorization.Users;
using AcmStatisticsBackend.Crawlers;
using AcmStatisticsBackend.Crawlers.Dto;
using AcmStatisticsBackend.ServiceClients;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace AcmStatisticsBackend.Tests.Accounts
{
    public class AccountAppService_Tests : AcmStatisticsBackendTestBase
    {
        private readonly IAccountAppService _accountAppService;
        private readonly FakeCaptchaServiceClient _captchaServiceClient;
        private readonly LogInManager _logInManager;

        public AccountAppService_Tests()
        {
            _captchaServiceClient = new FakeCaptchaServiceClient();
            _accountAppService = Resolve<AccountAppService>(new
            {
                captchaServiceClient = _captchaServiceClient,
            });
            _logInManager = Resolve<LogInManager>();
        }

        [Fact]
        public async Task Register_应该能够正确注册()
        {
            // arrange
            _captchaServiceClient.Return = new CaptchaServiceValidateResult
            {
                Correct = true,
                ErrorMessage = null,
            };

            // act
            var result = await _accountAppService.Register(new RegisterInput
            {
                UserName = "testuser",
                Password = "StrongPassword",
                CaptchaId = "aaaaa",
                CaptchaText = "fdafdsf",
            });

            // assert
            result.CanLogin.Should().BeTrue();

            await UsingDbContextAsync(async ctx =>
            {
                var user = await ctx.Users.FirstOrDefaultAsync(user => user.UserName == "testuser");
                user.EmailAddress.Should().Be("testuser@noemail.fake");
                user.IsEmailConfirmed.Should().BeFalse();
            });
        }

        [Fact]
        public async Task Register_能够在验证码错误时能报错()
        {
            // arrange
            _captchaServiceClient.Return = new CaptchaServiceValidateResult
            {
                Correct = false,
                ErrorMessage = "an error message",
            };

            // act
            await _accountAppService.Register(new RegisterInput
                {
                    UserName = "testuser",
                    Password = "StrongPassword",
                    CaptchaId = "aaaaa",
                    CaptchaText = "fdafdsf",
                }).ShouldThrow<UserFriendlyException>()
                // assert
                .WithMessage("an error message");
        }

        [Fact]
        public async Task SelfDelete_能够正确删除用户()
        {
            // arrange
            _captchaServiceClient.Return = new CaptchaServiceValidateResult
            {
                Correct = true,
                ErrorMessage = null,
            };
            await _accountAppService.Register(new RegisterInput
            {
                UserName = "user1",
                Password = "StrongPassword",
                CaptchaId = "a",
                CaptchaText = "a",
            });
            await UsingDbContextAsync(1, async ctx =>
            {
                var user = await ctx.Users.FirstAsync(a => a.UserName == "user1");
                user.Should().NotBeNull();
            });

            // act
            LoginAsTenant(AbpTenantBase.DefaultTenantName, "user1");
            await _accountAppService.SelfDelete();

            // test
            User origUser = null;
            await UsingDbContextAsync(1, async ctx =>
            {
                origUser = await ctx.Users.FirstOrDefaultAsync(a => a.UserName == "user1");
                origUser.IsDeleted.Should().BeTrue();
            });

            // 能够注册相同的用户名（和邮箱）
            await _accountAppService.Register(new RegisterInput
            {
                UserName = "user1",
                Password = "StrongPassword",
                CaptchaId = "a",
                CaptchaText = "a",
            });
            await UsingDbContextAsync(1, async ctx =>
            {
                var user = await ctx.Users.FirstAsync(a => a.UserName == "user1" && !a.IsDeleted);
                user.Should().NotBeNull();
                user.Id.Should().NotBe(origUser.Id);
            });
        }

        [Fact]
        public async Task ChangePassword_CanWorkCorrectly()
        {
            // arrange
            _captchaServiceClient.Return = new CaptchaServiceValidateResult
            {
                Correct = true,
                ErrorMessage = null,
            };
            await _accountAppService.Register(new RegisterInput
            {
                UserName = "user1",
                Password = "password1",
                CaptchaId = "a",
                CaptchaText = "a",
            });
            var result = await _logInManager.LoginAsync("user1", "password1", AbpTenantBase.DefaultTenantName);
            result.Result.Should().Be(AbpLoginResultType.Success);

            // act
            LoginAsTenant(AbpTenantBase.DefaultTenantName, "user1");
            await _accountAppService.ChangePassword(new ChangePasswordInput
            {
                CurrentPassword = "password1",
                NewPassword = "password2",
            });

            // assert
            (await _logInManager.LoginAsync("user1", "password2", AbpTenantBase.DefaultTenantName))
                .Result.Should().Be(AbpLoginResultType.Success);
            (await _logInManager.LoginAsync("user1", "password1", AbpTenantBase.DefaultTenantName))
                .Result.Should().Be(AbpLoginResultType.InvalidPassword);
        }
    }
}
