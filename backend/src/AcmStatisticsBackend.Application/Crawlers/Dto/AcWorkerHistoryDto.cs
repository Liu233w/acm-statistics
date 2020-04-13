using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using Abp.AutoMapper;

namespace AcmStatisticsBackend.Crawlers.Dto
{
    [AutoMap(typeof(AcWorkerHistory))]
    public class AcWorkerHistoryDto
    {
        public string CrawlerName { get; set; }

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
        [Range(0, int.MaxValue)]
        public int Submission { get; set; }

        /// <summary>
        /// 通过数
        /// </summary>
        [Range(0, int.MaxValue)]
        public int Solved { get; set; }

        /// <summary>
        /// 本记录是否包含通过题目列表。
        /// </summary>
        public bool HasSolvedList { get; set; }

        /// <summary>
        /// 用户通过的题目列表
        ///
        /// 如果 <see cref="HasSolvedList"/> 为false，返回一个空的列表。
        /// </summary>
        [Required]
        public string[] SolvedList { get; set; }
    }
}
