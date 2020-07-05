using System;
using System.ComponentModel.DataAnnotations;

namespace AcmStatisticsBackend.Crawlers.Dto
{
    public class GetQueryHistoryAndSummaryOutput
    {
        [Range(1, long.MaxValue)]
        public long HistoryId { get; set; }

        [Range(1, long.MaxValue)]
        public long SummaryId { get; set; }

        [Required]
        public DateTime CreationTime { get; set; }

        /// <summary>
        /// Submission count
        ///
        /// Is null if summary does not exist.
        /// </summary>
        public int? Submission { get; set; }

        /// <summary>
        /// Solved count
        ///
        /// Is null if summary does not exist.
        /// </summary>
        public int? Solved { get; set; }
    }
}
