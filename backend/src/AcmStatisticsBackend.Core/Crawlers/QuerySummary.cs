using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Abp.Domain.Entities;

namespace AcmStatisticsBackend.Crawlers
{
    /// <summary>
    /// The summary of a certain query.
    /// </summary>
    public class QuerySummary : Entity<long>
    {
        [Required]
        public QueryHistory QueryHistory { get; set; }

        public long QueryHistoryId { get; set; }

        /// <summary>
        /// Query summaries of each crawler.
        /// </summary>
        [Required]
        public ICollection<QueryCrawlerSummary> QueryCrawlerSummaries { get; set; }

        /// <summary>
        /// Warnings in summary generation.
        /// </summary>
        [Required]
        public ICollection<string> SummaryWarnings { get; set; }

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
