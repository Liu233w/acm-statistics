using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Abp.Domain.Entities.Auditing;
using AcmStatisticsBackend.Authorization.Users;

namespace AcmStatisticsBackend.Crawlers
{
    /// <summary>
    /// 用户的默认查询。用户登录后，查题页面会自动填充此查询的内容
    /// </summary>
    public class DefaultQuery : FullAuditedEntity<long, User>
    {
        [Required]
        public User User { get; set; }

        public long UserId { get; set; }

        /// <summary>
        /// 主用户名
        /// </summary>
        [Required]
        [MinLength(0)]
        public string MainUsername { get; set; }

        /// <summary>
        /// 在各个爬虫上的用户名。key为爬虫名称，value为一个用户名的列表，表示在该爬虫上的所有用户名。
        /// </summary>
        [Required]
        public Dictionary<string, List<string>> UsernamesInCrawlers { get; set; }
    }
}
