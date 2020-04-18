using System.Threading.Tasks;
using Abp.Authorization;
using Abp.MultiTenancy;
using Abp.UI;
using AcmStatisticsBackend.Accounts;
using AcmStatisticsBackend.Accounts.Dto;
using AcmStatisticsBackend.Authorization;
using AcmStatisticsBackend.Authorization.Users;
using AcmStatisticsBackend.ServiceClients;
using Microsoft.EntityFrameworkCore;
using Shouldly;
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
            result.CanLogin.ShouldBe(true);

            await UsingDbContextAsync(async ctx =>
            {
                var user = await ctx.Users.FirstOrDefaultAsync(user => user.UserName == "testuser");
                user.EmailAddress.ShouldBe("testuser@noemail.fake");
                user.IsEmailConfirmed.ShouldBe(false);
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
            var error = await _accountAppService.Register(new RegisterInput
            {
                UserName = "testuser",
                Password = "StrongPassword",
                CaptchaId = "aaaaa",
                CaptchaText = "fdafdsf",
            }).ShouldThrowAsync<UserFriendlyException>();

            // assert
            error.Message.ShouldBe("an error message");
        }

        // TODO: 等到以后有邮箱选项的时候，记得测试这里会不会冲突
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
                user.ShouldNotBeNull();
            });

            // act
            LoginAsTenant(AbpTenantBase.DefaultTenantName, "user1");
            await _accountAppService.SelfDelete();

            // test
            User origUser = null;
            await UsingDbContextAsync(1, async ctx =>
            {
                origUser = await ctx.Users.FirstOrDefaultAsync(a => a.UserName == "user1");
                origUser.IsDeleted.ShouldBe(true);
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
                user.ShouldNotBeNull();
                user.Id.ShouldNotBe(origUser.Id);
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
            result.Result.ShouldBe(AbpLoginResultType.Success);

            // act
            LoginAsTenant(AbpTenantBase.DefaultTenantName, "user1");
            await _accountAppService.ChangePassword(new ChangePasswordInput
            {
                CurrentPassword = "password1",
                NewPassword = "password2",
            });

            // assert
            (await _logInManager.LoginAsync("user1", "password2", AbpTenantBase.DefaultTenantName))
                .Result.ShouldBe(AbpLoginResultType.Success);
            (await _logInManager.LoginAsync("user1", "password1", AbpTenantBase.DefaultTenantName))
                .Result.ShouldBe(AbpLoginResultType.InvalidPassword);
        }
    }
}
