using System.Threading.Tasks;
using Abp.Dependency;
using Flurl.Http;

namespace AcmStatisticsBackend.ServiceClients
{
    public class CaptchaServiceClient : ICaptchaServiceClient, ISingletonDependency
    {
        /// <inheritdoc cref="ICaptchaServiceClient.ValidateAsync"/>
        public async Task<CaptchaServiceValidateResult> ValidateAsync(string id, string text)
        {
            var response = await "http://captcha-service"
                .PostUrlEncodedAsync(new
                {
                    id = id,
                    text = text,
                })
                .ReceiveJson<CaptchaServiceValidateRestResponse>();

            var result = new CaptchaServiceValidateResult
            {
                Correct = !response.error,
            };
            if (response.error)
            {
                result.ErrorMessage = response.message;
            }

            return result;
        }

        private class CaptchaServiceValidateRestResponse
        {
            // rest 里不是大写
#pragma warning disable SA1300
            public bool error { get; set; }
            public string message { get; set; }
#pragma warning restore SA1300
        }
    }
}
