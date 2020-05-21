using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using Abp.AutoMapper;
using Abp.Runtime.Validation;

namespace AcmStatisticsBackend.Crawlers.Dto
{
    [AutoMap(typeof(QueryWorkerHistory))]
    public class QueryWorkerHistoryDto : ICustomValidate
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
        /// Whether this record has a list of problem ids that user solved.
        /// </summary>
        public bool HasSolvedList { get; set; }

        /// <summary>
        /// The list of problem ids that user solved.
        ///
        /// If <see cref="HasSolvedList"/> is false, an empty list is returned.
        /// </summary>
        public string[] SolvedList { get; set; }

        /// <summary>
        /// Whether current crawler is virtual judge.
        /// </summary>
        public bool IsVirtualJudge { get; set; } = false;

        /// <summary>
        /// If <see cref="IsVirtualJudge"/> is false, this field is empty.
        /// Otherwise, this field contains submissions count in each crawler.
        /// </summary>
        public Dictionary<string, int> SubmissionsByCrawlerName { get; set; } = new Dictionary<string, int>();

        public void AddValidationErrors(CustomValidationContext context)
        {
            if (ErrorMessage == null && HasSolvedList && SolvedList == null)
            {
                context.Results.Add(new ValidationResult("SolvedList should not be null when HasSolvedList is true."));
            }
        }
    }
}
