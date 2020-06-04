using System.Collections.Generic;
using System.Threading.Tasks;
using AcmStatisticsBackend.ServiceClients;

namespace AcmStatisticsBackend.Tests.DependencyInjection
{
    public class TestCrawlerApiBackendClient : ICrawlerApiBackendClient
    {
        public List<CrawlerMetaItem> CrawlerMeta { get; set; }

#pragma warning disable 1998
        public async Task<IReadOnlyCollection<CrawlerMetaItem>> GetCrawlerMeta()
        {
            return CrawlerMeta.AsReadOnly();
        }
#pragma warning restore 1998
    }
}
