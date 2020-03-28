using System.Threading.Tasks;
using AcmStatisticsBackend.ServiceClients;

namespace AcmStatisticsBackend.Tests.Accounts
{
    public class FakeCaptchaServiceClient : ICaptchaServiceClient
    {
        public CaptchaServiceValidateResult Return { get; set; }

        public Task<CaptchaServiceValidateResult> ValidateAsync(string id, string text)
        {
            return Task.FromResult(Return);
        }
    }
}
