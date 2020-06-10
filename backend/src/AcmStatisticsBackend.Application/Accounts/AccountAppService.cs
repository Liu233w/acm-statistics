using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Abp.Authorization;
using Abp.IdentityFramework;
using Abp.Runtime.Session;
using Abp.UI;
using AcmStatisticsBackend.Accounts.Dto;
using AcmStatisticsBackend.Authorization;
using AcmStatisticsBackend.Authorization.Users;
using AcmStatisticsBackend.ServiceClients;
using Microsoft.AspNetCore.Identity;

namespace AcmStatisticsBackend.Accounts
{
    public class AccountAppService : AcmStatisticsBackendAppServiceBase, IAccountAppService
    {
        private readonly UserRegistrationManager _userRegistrationManager;
        private readonly ICaptchaServiceClient _captchaServiceClient;
        private readonly UserManager _userManager;
        private readonly IAbpSession _abpSession;
        private readonly LogInManager _logInManager;
        private readonly IPasswordHasher<User> _passwordHasher;

        public AccountAppService(
            UserRegistrationManager userRegistrationManager, ICaptchaServiceClient captchaServiceClient,
            UserManager userManager, IAbpSession abpSession, LogInManager logInManager,
            IPasswordHasher<User> passwordHasher)
        {
            _userRegistrationManager = userRegistrationManager;
            _captchaServiceClient = captchaServiceClient;
            _userManager = userManager;
            _abpSession = abpSession;
            _logInManager = logInManager;
            _passwordHasher = passwordHasher;
        }

        public async Task<RegisterOutput> Register(RegisterInput input)
        {
            var captchaResult = await _captchaServiceClient.ValidateAsync(input.CaptchaId, input.CaptchaText);
            if (!captchaResult.Correct)
            {
                throw new UserFriendlyException(captchaResult.ErrorMessage);
            }

            await _userRegistrationManager.RegisterAsync(
                input.UserName,
                input.Password);

            return new RegisterOutput
            {
                CanLogin = true,
            };
        }

        /// <inheritdoc />
        [AbpAuthorize]
        public async Task SelfDelete()
        {
            var user = await _userManager.GetUserByIdAsync(_abpSession.GetUserId());
            var identityResult = await _userManager.DeleteAsync(user);
            identityResult.CheckErrors();
        }

        /// <inheritdoc />
        [AbpAuthorize]
        public async Task ChangePassword(ChangePasswordInput input)
        {
            Debug.Assert(_abpSession.UserId != null, "_abpSession.UserId != null");
            var userId = _abpSession.UserId.Value;
            var user = await _userManager.GetUserByIdAsync(userId);
            var loginAsync = await _logInManager.LoginAsync(user.UserName, input.CurrentPassword, shouldLockout: false);
            if (loginAsync.Result != AbpLoginResultType.Success)
            {
                throw new UserFriendlyException(
                    "Your 'Existing Password' did not match the one on record.  Please try again or contact an administrator for assistance in resetting your password.");
            }

            user.Password = _passwordHasher.HashPassword(user, input.NewPassword);
        }
    }
}
