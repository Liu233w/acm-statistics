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
    public class QueryHistory : Entity<long>
    {
        [Required]
        public DateTime CreationTime { get; set; }

        /// <summary>
        /// The user related to this entity
        /// </summary>
        public User User { get; set; }

        public long UserId { get; set; }

        /// <summary>
        /// Main username of query history, can be empty
        /// </summary>
        [Required]
        [MinLength(0)]
        public string MainUsername { get; set; }

        /// <summary>
        /// Total submission count
        /// </summary>
        [Range(0, int.MaxValue)]
        public int Submission { get; set; }

        /// <summary>
        /// Total solved count, redundant problems (including problems in virtual_judge) are removed.
        /// </summary>
        [Range(0, int.MaxValue)]
        public int Solved { get; set; }

        /// <summary>
        /// Query history of each crawler.
        /// </summary>
        [Required]
        public ICollection<QueryWorkerHistory> QueryWorkerHistories { get; set; }
    }
}
