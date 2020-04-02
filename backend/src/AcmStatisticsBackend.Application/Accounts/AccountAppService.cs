using System.Threading.Tasks;
using Abp.Authorization;
using Abp.IdentityFramework;
using Abp.Runtime.Session;
using Abp.UI;
using AcmStatisticsBackend.Accounts.Dto;
using AcmStatisticsBackend.Authorization.Users;
using AcmStatisticsBackend.ServiceClients;

namespace AcmStatisticsBackend.Accounts
{
    public class AccountAppService : AcmStatisticsBackendAppServiceBase, IAccountAppService
    {
        // from: http://regexlib.com/REDetails.aspx?regexp_id=1923
        public const string PasswordRegex =
            "(?=^.{8,}$)(?=.*\\d)(?=.*[a-z])(?=.*[A-Z])(?!.*\\s)[0-9a-zA-Z!@#$%^&*()]*$";

        private readonly UserRegistrationManager _userRegistrationManager;

        private readonly ICaptchaServiceClient _captchaServiceClient;

        private readonly UserManager _userManager;

        private readonly IAbpSession _abpSession;

        public AccountAppService(
            UserRegistrationManager userRegistrationManager, ICaptchaServiceClient captchaServiceClient, UserManager userManager, IAbpSession abpSession)
        {
            _userRegistrationManager = userRegistrationManager;
            _captchaServiceClient = captchaServiceClient;
            _userManager = userManager;
            _abpSession = abpSession;
        }

        public async Task<RegisterOutput> Register(RegisterInput input)
        {
            var captchaResult = await _captchaServiceClient.ValidateAsync(input.CaptchaId, input.CaptchaText);
            if (!captchaResult.Correct)
            {
                throw new UserFriendlyException(captchaResult.ErrorMessage);
            }

            var user = await _userRegistrationManager.RegisterAsync(
                input.UserName,
                input.Password);

            return new RegisterOutput
            {
                CanLogin = true,
            };
        }

        /// <summary>
        /// É¾³ý±¾ÕË»§
        /// </summary>
        [AbpAuthorize]
        public async Task SelfDelete()
        {
            var user = await _userManager.GetUserByIdAsync(_abpSession.GetUserId());
            var identityResult = await _userManager.DeleteAsync(user);
            identityResult.CheckErrors();
        }
    }
}
