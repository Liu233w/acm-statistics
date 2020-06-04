using System.Net.Http;
using System.Threading.Tasks;
using AcmStatisticsBackend.ServiceClients;
using FluentAssertions;
using Flurl.Http.Testing;
using Xunit;

namespace AcmStatisticsBackend.Tests.ServiceClients
{
    public class CaptchaServiceClient_Tests
    {
        private readonly ICaptchaServiceClient _captchaServiceClient;

        public CaptchaServiceClient_Tests()
        {
            _captchaServiceClient = new CaptchaServiceClient();
        }

        [Fact]
        public async Task WhenHttpResponseWithoutError_ShouldWorkCorrectly()
        {
            // arrange
            using var httpTest = new HttpTest();
            httpTest.RespondWithJson(new
            {
                error = false,
            });

            // act
            var result = await _captchaServiceClient.ValidateAsync("id", "text");

            // assert
            httpTest.ShouldHaveCalled("http://captcha-service/api/captcha-service/validate")
                .WithVerb(HttpMethod.Post)
                .WithRequestUrlEncoded(new
                {
                    id = "id",
                    text = "text",
                })
                .Times(1);

            result.Correct.Should().BeTrue();
            result.ErrorMessage.Should().BeEmpty();
        }

        [Fact]
        public async Task WhenHttpResponseError_ShouldReportError()
        {
            // arrange
            using var httpTest = new HttpTest();
            httpTest.RespondWithJson(new
            {
                error = true,
                message = "err message",
            }, 400);

            // act
            var result = await _captchaServiceClient.ValidateAsync("id", "text");

            // assert
            httpTest.ShouldHaveCalled("http://captcha-service/api/captcha-service/validate")
                .WithVerb(HttpMethod.Post)
                .WithRequestUrlEncoded(new
                {
                    id = "id",
                    text = "text",
                })
                .Times(1);

            result.Correct.Should().BeFalse();
            result.ErrorMessage.Should().Be("err message");
        }
    }
}
