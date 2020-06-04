using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Abp.AutoMapper;

namespace AcmStatisticsBackend.Crawlers.Dto
{
    [AutoMap(typeof(QuerySummary))]
    public class QuerySummaryDto
    {
        public long QueryHistoryId { get; set; }

        /// <summary>
        /// When the summary is generated
        /// </summary>
        public DateTime GenerateTime { get; set; }

        /// <summary>
        /// Query summaries of each crawler.
        /// </summary>
        [Required]
        public ICollection<QueryCrawlerSummaryDto> QueryCrawlerSummaries { get; set; }

        /// <summary>
        /// Warnings in summary generation.
        /// </summary>
        [Required]
        public ICollection<SummaryWarning> SummaryWarnings { get; set; }

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
    }
}
