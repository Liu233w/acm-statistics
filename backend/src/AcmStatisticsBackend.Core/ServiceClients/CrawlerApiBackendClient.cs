using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using Abp.Dependency;
using Flurl.Http;

namespace AcmStatisticsBackend.ServiceClients
{
    public class CrawlerApiBackendClient : ICrawlerApiBackendClient, ISingletonDependency
    {
        private IReadOnlyCollection<CrawlerMetaItem> _cachedMeta = null;

        public async Task<IReadOnlyCollection<CrawlerMetaItem>> GetCrawlerMeta()
        {
            if (_cachedMeta == null)
            {
                var res = await "http://crawler-api-backend/api/crawlers/"
                    .GetJsonAsync<GetMetaReturn>();

                _cachedMeta = res.data
                    .Select(pair => new CrawlerMetaItem
                    {
                        CrawlerName = pair.Key,
                        CrawlerTitle = pair.Value.title,
                        CrawlerDescription = pair.Value.description,
                        Url = pair.Value.url,
                        IsVirtualJudge = pair.Value.virtual_judge == true,
                    })
                    .ToList();
            }

            return _cachedMeta;
        }

        // Not capitalized in json
#pragma warning disable SA1300
        private class GetMetaReturn
        {
            public bool error { get; set; }

            public IDictionary<string, DataItem> data { get; set; }
        }

        private class DataItem
        {
            public string title { get; set; }
            public string description { get; set; }
            public string url { get; set; }
            public bool? virtual_judge { get; set; }
        }
#pragma warning restore SA1300
    }
}
