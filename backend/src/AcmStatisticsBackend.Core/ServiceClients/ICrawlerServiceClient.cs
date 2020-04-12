using System.Collections.Generic;
using System.Threading.Tasks;

namespace AcmStatisticsBackend.ServiceClients
{
    /// <summary>
    /// 用来访问爬虫后端的 Client
    /// </summary>
    public interface ICrawlerServiceClient
    {
        /// <summary>
        /// 获取爬虫的元数据
        /// </summary>
        /// <returns>一个爬虫元数据列表，每个元素包含了爬虫名称等信息</returns>
        Task<ICollection<CrawlerMetaResult>> GetCrawlerMetaAsync();

        // TODO: 调用爬虫
        // 可以使用函数式库里的 Either，来表示成功或失败
    }
}
