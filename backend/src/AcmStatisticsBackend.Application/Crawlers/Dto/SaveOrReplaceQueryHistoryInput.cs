using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Abp.AutoMapper;

namespace AcmStatisticsBackend.Crawlers.Dto
{
    [AutoMapTo(typeof(QueryHistory))]
    public class SaveOrReplaceQueryHistoryInput
    {
        /// <summary>
        /// Main username of query history, can be empty
        /// </summary>
        [Required]
        [MinLength(0)]
        public string MainUsername { get; set; }

        /// <summary>
        /// Query history of each crawler.
        /// </summary>
        public ICollection<QueryWorkerHistoryDto> QueryWorkerHistories { get; set; }
    }
}
