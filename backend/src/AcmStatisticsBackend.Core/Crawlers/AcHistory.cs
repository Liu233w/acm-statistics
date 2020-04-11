using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using AcmStatisticsBackend.Authorization.Users;

namespace AcmStatisticsBackend.Crawlers
{
    /// <summary>
    /// 一次查询历史记录
    /// </summary>
    public class AcHistory : Entity<long>, IHasCreationTime
    {
        public DateTime CreationTime { get; set; }

        /// <summary>
        /// 跟此记录关联的用户
        /// </summary>
        public virtual User User { get; set; }

        public long UserId { get; set; }

        /// <summary>
        /// 查询历史记录中的主用户名，可以为空字符串。
        /// </summary>
        [Required]
        [MinLength(0)]
        public string MainUsername { get; set; }

        /// <summary>
        /// 用户的总提交数
        /// </summary>
        public int Submission { get; set; }

        /// <summary>
        /// 用户的总通过数，已经移除了 vjudge 中重复的题目
        /// </summary>
        public int Solved { get; set; }

        /// <summary>
        /// 包含在本次查询记录中的，每个爬虫的查询记录
        /// </summary>
        [Required]
        public virtual ICollection<AcWorkerHistory> AcWorkerHistories { get; set; }
    }
}
