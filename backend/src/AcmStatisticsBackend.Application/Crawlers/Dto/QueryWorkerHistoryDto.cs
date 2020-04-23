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
        /// <inheritdoc cref="QueryWorkerHistory.CrawlerName"/>
        [Required]
        public string CrawlerName { get; set; }

        /// <inheritdoc cref="QueryWorkerHistory.Username"/>
        [Required]
        public string Username { get; set; }

        /// <inheritdoc cref="QueryWorkerHistory.ErrorMessage"/>
        [MaybeNull]
        public string ErrorMessage { get; set; }

        /// <inheritdoc cref="QueryWorkerHistory.Submission"/>
        [Range(0, int.MaxValue)]
        public int Submission { get; set; }

        /// <inheritdoc cref="QueryWorkerHistory.Solved"/>
        [Range(0, int.MaxValue)]
        public int Solved { get; set; }

        /// <inheritdoc cref="QueryWorkerHistory.HasSolvedList"/>
        public bool HasSolvedList { get; set; }

        /// <inheritdoc cref="QueryWorkerHistory.SolvedList"/>
        public string[] SolvedList { get; set; }

        /// <inheritdoc cref="QueryWorkerHistory.IsVirtualJudge"/>
        public bool IsVirtualJudge { get; set; } = false;

        /// <inheritdoc cref="QueryWorkerHistory.SubmissionsByCrawlerName"/>
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
