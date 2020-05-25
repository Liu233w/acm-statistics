using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Abp.Domain.Entities;

namespace AcmStatisticsBackend.Crawlers
{
    /// <summary>
    /// Store the summary of a certain crawler.
    /// </summary>
    public class QueryCrawlerSummary : Entity<long>
    {
        [Required]
        public QuerySummary QuerySummary { get; set; }

        public long QuerySummaryId { get; set; }

        /// <summary>
        /// The name of the crawler. Frontend can get its title by this field.
        /// </summary>
        [Required]
        public string CrawlerName { get; set; }

        /// <summary>
        /// Submission count.
        /// </summary>
        [Range(0, int.MaxValue)]
        public int Submission { get; set; }

        /// <summary>
        /// Solved count.
        /// </summary>
        [Range(0, int.MaxValue)]
        public int Solved { get; set; }

        /// <summary>
        /// Usernames used in this crawler
        /// </summary>
        public ICollection<UsernameInCrawler> Usernames { get; set; }

        public bool IsVirtualJudge { get; set; }
    }
}
