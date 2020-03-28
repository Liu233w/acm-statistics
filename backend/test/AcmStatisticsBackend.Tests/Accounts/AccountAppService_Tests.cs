using System.Threading.Tasks;
using Abp.UI;
using AcmStatisticsBackend.Accounts;
using AcmStatisticsBackend.Accounts.Dto;
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

        public AccountAppService_Tests()
        {
            _captchaServiceClient = new FakeCaptchaServiceClient();
            _accountAppService = Resolve<AccountAppService>(new
            {
                captchaServiceClient = _captchaServiceClient,
            });
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
                user.EmailAddress.ShouldBe("");
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
    }
}
