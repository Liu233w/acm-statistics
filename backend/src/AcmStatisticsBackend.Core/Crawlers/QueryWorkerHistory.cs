using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using Abp.Domain.Entities;

namespace AcmStatisticsBackend.Crawlers
{
    /// <summary>
    /// Query history in a certain crawler
    /// </summary>
    public class QueryWorkerHistory : Entity<long>
    {
        /// <summary>
        /// QueryHistory the entity related to
        /// </summary>
        [Required]
        public QueryHistory QueryHistory { get; set; }

        public long QueryHistoryId { get; set; }

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
        /// Whether current crawler is virtual judge when the history is submitted.
        /// </summary>
        public bool IsVirtualJudge { get; set; }

        /// <summary>
        /// If <see cref="IsVirtualJudge"/> is false, this field is null.
        /// Otherwise, this field contains submissions count in each crawler.
        /// </summary>
        [MaybeNull]
        public IDictionary<string, int> SubmissionsByCrawlerName { get; set; }
    }
}
