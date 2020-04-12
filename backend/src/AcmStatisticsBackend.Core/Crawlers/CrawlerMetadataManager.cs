using System.Threading.Tasks;
using Abp.Dependency;
using Abp.Domain.Repositories;
using AcmStatisticsBackend.ServiceClients;

namespace AcmStatisticsBackend.Crawlers
{
    /// <summary>
    /// 用来管理爬虫的元信息
    /// </summary>
    public class CrawlerMetadataManager : ITransientDependency
    {
        private readonly ICrawlerServiceClient _crawlerServiceClient;

        private readonly IRepository<OjCrawler> _ojCrawlerRepository;

        public CrawlerMetadataManager(ICrawlerServiceClient crawlerServiceClient,
            IRepository<OjCrawler> ojCrawlerRepository)
        {
            _crawlerServiceClient = crawlerServiceClient;
            _ojCrawlerRepository = ojCrawlerRepository;
        }

        /// <summary>
        /// 从爬虫后端更新爬虫的元数据
        /// </summary>
        public async Task UpdateCrawlerMetadataAsync()
        {
            var crawlerMeta = await _crawlerServiceClient.GetCrawlerMetaAsync();

            foreach (var item in crawlerMeta)
            {
                if (await _ojCrawlerRepository.FirstOrDefaultAsync(e => e.CrawlerName == item.Name) == null)
                {
                    await _ojCrawlerRepository.InsertAsync(new OjCrawler
                    {
                        Title = item.Title,
                        CrawlerName = item.Name,
                    });
                }
            }
        }

        /// <summary>
        /// 根据爬虫名称获取爬虫元数据对象
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public async Task<OjCrawler> GetEntityByNameAsync(string name)
        {
            
            // TODO: PERF: 设定一定的时间间隔，避免短时间内多次更新
        }
    }
}
