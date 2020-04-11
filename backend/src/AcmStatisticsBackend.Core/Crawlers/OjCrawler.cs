using System.ComponentModel.DataAnnotations;
using Abp.Domain.Entities;

namespace AcmStatisticsBackend.Crawlers
{
    /// <summary>
    /// 表示一个OJ的爬虫
    /// </summary>
    public class OjCrawler : Entity
    {
        /// <summary>
        /// 爬虫的名称。和 /crawler/config.yml 中爬虫的名称相同
        /// </summary>
        [Required]
        [MinLength(1)]
        public string CrawlerName { get; set; }

        /// <summary>
        /// 爬虫的标题
        /// </summary>
        [Required]
        [MinLength(1)]
        public string Title { get; set; }
    }
}
