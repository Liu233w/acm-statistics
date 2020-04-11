using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using Abp.Domain.Entities;

namespace AcmStatisticsBackend.Crawlers
{
    /// <summary>
    /// 用户在单个爬虫上的一次历史记录
    /// </summary>
    public class AcWorkerHistory : Entity<long>
    {
        /// <summary>
        /// 关联的 AcHistory
        /// </summary>
        [Required]
        public virtual AcHistory AcHistory { get; set; }

        public long AcHistoryId { get; set; }

        /// <summary>
        /// 关联的爬虫信息，可以从中获取爬虫的标题等数据
        /// </summary>
        [Required]
        public virtual OjCrawler OjCrawler { get; set; }

        public int OjCrawlerId { get; set; }

        /// <summary>
        /// 用户用来查询的用户名
        /// </summary>
        [Required]
        public string Username { get; set; }

        /// <summary>
        /// 爬虫的错误信息。如果不为null，表示本次查询失败，这时 <see cref="Submission"/> 和
        /// <see cref="Solved"/> 都为0。
        /// </summary>
        [MaybeNull]
        public string ErrorMessage { get; set; }

        /// <summary>
        /// 提交数
        /// </summary>
        public int Submission { get; set; }

        /// <summary>
        /// 通过数
        /// </summary>
        public int Solved { get; set; }

        /// <summary>
        /// 本记录是否包含通过题目列表。
        /// </summary>
        public bool HasSolvedList { get; set; }

        /// <summary>
        /// 用户通过的题目列表，用json形式存储。
        ///
        /// 如果 <see cref="HasSolvedList"/> 为false，返回一个空的列表。
        /// </summary>
        [Required]
        public JsonObject<string[]> SolvedListJson { get; set; }
    }
}
