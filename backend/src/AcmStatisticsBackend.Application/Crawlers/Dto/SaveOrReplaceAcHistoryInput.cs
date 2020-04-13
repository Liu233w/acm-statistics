using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Abp.AutoMapper;

namespace AcmStatisticsBackend.Crawlers.Dto
{
    [AutoMapTo(typeof(AcHistory))]
    public class SaveOrReplaceAcHistoryInput
    {
        /// <summary>
        /// 查询历史记录中的主用户名，可以为空字符串。
        /// </summary>
        [Required]
        [MinLength(0)]
        public string MainUsername { get; set; }

        /// <summary>
        /// 用户的总提交数
        /// </summary>
        [Range(0, int.MaxValue)]
        public int Submission { get; set; }

        /// <summary>
        /// 用户的总通过数，已经移除了 vjudge 中重复的题目
        /// </summary>
        [Range(0, int.MaxValue)]
        public int Solved { get; set; }

        public ICollection<AcWorkerHistoryDto> AcWorkerHistories { get; set; }
    }
}
