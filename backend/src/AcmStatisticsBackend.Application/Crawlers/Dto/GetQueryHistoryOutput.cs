using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Abp.AutoMapper;

namespace AcmStatisticsBackend.Crawlers.Dto
{
    [AutoMap(typeof(QueryHistory))]
    public class GetQueryHistoryOutput
    {
        [Range(1, long.MaxValue)]
        public long Id { get; set; }

        [Required]
        public DateTime CreationTime { get; set; }

        /// <summary>
        /// Main username of query history, can be null or empty
        /// </summary>
        public string MainUsername { get; set; }
    }
}
