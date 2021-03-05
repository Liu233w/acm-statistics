using System;
using System.ComponentModel.DataAnnotations;

namespace AcmStatisticsBackend.Crawlers.Dto
{
    public class GetQueryHistoryAndSummaryOutput
    {
        [Range(1, long.MaxValue)]
        public long HistoryId { get; set; }

        /// <summary>
        /// The id of the summary.
        ///
        /// It can be null if the summary does not exist
        /// </summary>
        [Range(1, long.MaxValue)]
        public long? SummaryId { get; set; }

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
