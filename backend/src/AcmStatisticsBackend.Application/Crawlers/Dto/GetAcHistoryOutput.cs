using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Abp.AutoMapper;

namespace AcmStatisticsBackend.Crawlers.Dto
{
    [AutoMap(typeof(AcHistory))]
    public class GetAcHistoryOutput
    {
        public long Id { get; set; }

        [Required]
        public DateTime CreationTime { get; set; }

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
    }
}
