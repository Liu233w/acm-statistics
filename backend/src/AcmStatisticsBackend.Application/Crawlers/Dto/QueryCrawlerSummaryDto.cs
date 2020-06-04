using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using AutoMapper;

namespace AcmStatisticsBackend.Crawlers.Dto
{
    [AutoMap(typeof(QueryCrawlerSummary))]
    public class QueryCrawlerSummaryDto
    {
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
        public ICollection<UsernameInCrawlerDto> Usernames { get; set; }

        public bool IsVirtualJudge { get; set; }
    }
}
