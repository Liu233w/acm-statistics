using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using Abp.AutoMapper;

namespace AcmStatisticsBackend.Crawlers.Dto
{
    [AutoMap(typeof(QueryWorkerHistory))]
    public class QueryWorkerHistoryDto
    {
        /// <summary>
        /// The name of the crawler. Frontend can get its title by this field.
        /// </summary>
        [Required]
        public string CrawlerName { get; set; }

        /// <summary>
        /// The username used to query this crawler.
        /// </summary>
        [Required]
        public string Username { get; set; }

        /// <summary>
        /// Error message of the crawler. If it's not null, current query is failed, and
        /// <see cref="Submission"/> and <see cref="Solved"/> are all 0.
        /// </summary>
        [MaybeNull]
        public string ErrorMessage { get; set; }

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
        /// The list of problem ids that user solved.
        ///
        /// Can be null if crawler does not support it.
        /// </summary>
        [MaybeNull]
        public string[] SolvedList { get; set; }

        /// <summary>
        /// Whether current crawler is virtual judge.
        /// </summary>
        public bool IsVirtualJudge { get; set; }

        /// <summary>
        /// If <see cref="IsVirtualJudge"/> is false, this field is null.
        /// Otherwise, this field contains submissions count in each crawler.
        /// </summary>
        [MaybeNull]
        public Dictionary<string, int> SubmissionsByCrawlerName { get; set; } = new Dictionary<string, int>();
    }
}
