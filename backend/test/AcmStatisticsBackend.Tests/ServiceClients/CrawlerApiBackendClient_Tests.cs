using System.Net.Http;
using System.Threading.Tasks;
using AcmStatisticsBackend.Crawlers;
using AcmStatisticsBackend.ServiceClients;
using FluentAssertions;
using Flurl.Http.Testing;
using Xunit;

namespace AcmStatisticsBackend.Tests.ServiceClients
{
    public class CrawlerApiBackendClient_Tests
    {
        private readonly ICrawlerApiBackendClient _crawlerApiBackendClient;

        public CrawlerApiBackendClient_Tests()
        {
            _crawlerApiBackendClient = new CrawlerApiBackendClient();
        }

        [Fact]
        public async Task GetCrawlerMeta_ShouldWorkCorrectly()
        {
            // arrange
            using var httpTest = new HttpTest();

            httpTest.RespondWithJson(new
            {
                error = false,
                data = new
                {
                    uva = new
                    {
                        title = "UVA",
                        description = "u description",
                        url = "http",
                    },
                    vjudge = new
                    {
                        title = "VJudge",
                        description = "v description",
                        url = "https",
                        virtual_judge = true,
                    },
                },
            });

            // act
            var result = await _crawlerApiBackendClient.GetCrawlerMeta();

            // assert
            httpTest.ShouldHaveCalled("http://crawler-api-backend/api/crawlers/")
                .WithVerb(HttpMethod.Get)
                .Times(1);

            result.Should().BeEquivalentTo(new[]
            {
                new CrawlerMetaItem
                {
                    CrawlerName = "uva",
                    CrawlerTitle = "UVA",
                    CrawlerDescription = "u description",
                    Url = "http",
                    IsVirtualJudge = false,
                },
                new CrawlerMetaItem
                {
                    CrawlerName = "vjudge",
                    CrawlerTitle = "VJudge",
                    CrawlerDescription = "v description",
                    Url = "https",
                    IsVirtualJudge = true,
                },
            });
        }
    }
}
