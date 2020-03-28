using System.Threading.Tasks;
using Abp.UI;
using AcmStatisticsBackend.Authorization.Accounts.Dto;
using AcmStatisticsBackend.Authorization.Users;
using AcmStatisticsBackend.ServiceClients;

namespace AcmStatisticsBackend.Authorization.Accounts
{
    public class AccountAppService : AcmStatisticsBackendAppServiceBase, IAccountAppService
    {
        // from: http://regexlib.com/REDetails.aspx?regexp_id=1923
        public const string PasswordRegex =
            "(?=^.{8,}$)(?=.*\\d)(?=.*[a-z])(?=.*[A-Z])(?!.*\\s)[0-9a-zA-Z!@#$%^&*()]*$";

        private readonly UserRegistrationManager _userRegistrationManager;

        private readonly ICaptchaServiceClient _captchaServiceClient;

        public AccountAppService(
            UserRegistrationManager userRegistrationManager, ICaptchaServiceClient captchaServiceClient)
        {
            _userRegistrationManager = userRegistrationManager;
            _captchaServiceClient = captchaServiceClient;
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
    }
}
