using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading.Tasks;

namespace AcmStatisticsBackend.ServiceClients
{
    public interface ICrawlerApiBackendClient
    {
        /// <summary>
        /// Get the meta data of crawlers
        /// </summary>
        Task<IReadOnlyCollection<CrawlerMetaItem>> GetCrawlerMeta();
    }
}
