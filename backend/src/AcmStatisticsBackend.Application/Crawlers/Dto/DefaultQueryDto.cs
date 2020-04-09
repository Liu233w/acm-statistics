using System.Collections.Generic;
using Abp.AutoMapper;

namespace AcmStatisticsBackend.Crawlers.Dto
{
    /// <summary>
    /// 用户的默认爬虫用户名
    /// </summary>
    [AutoMap(typeof(DefaultQuery))]
    public class DefaultQueryDto
    {
        /// <summary>
        /// 主用户名
        /// </summary>
        public string MainUsername { get; set; }

        /// <summary>
        /// 在各个爬虫上的用户名。key为爬虫名称，value为一个用户名的列表，表示在该爬虫上的所有用户名。
        /// </summary>
        public Dictionary<string, List<string>> UsernamesInCrawlers { get; set; }
    }
}
