using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Abp.Domain.Entities;
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
        /// Query history of each crawler.
        /// </summary>
        [Required]
        public ICollection<QueryWorkerHistory> QueryWorkerHistories { get; set; }

        /// <summary>
        /// Is data source reliable (solved/submission are really from certain username)
        ///
        /// When get the history from user directly, it should be false;
        /// when get it from crawler-api-backend, it should be true.
        /// </summary>
        public bool IsReliableSource { get; set; } = false;

        /// <summary>
        /// The summary of this query history.
        ///
        /// Can be empty if summary is not generated.
        /// </summary>
        public QuerySummary QuerySummary { get; set; }

        public long? QuerySummaryId { get; set; }
    }
}
