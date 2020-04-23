﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Abp.AutoMapper;

namespace AcmStatisticsBackend.Crawlers.Dto
{
    [AutoMap(typeof(QueryHistory))]
    public class GetQueryHistoryOutput
    {
        public long Id { get; set; }

        [Required]
        public DateTime CreationTime { get; set; }

        /// <inheritdoc cref="QueryHistory.MainUsername"/>
        [Required]
        [MinLength(0)]
        public string MainUsername { get; set; }

        /// <inheritdoc cref="QueryHistory.Submission"/>
        [Range(0, int.MaxValue)]
        public int Submission { get; set; }

        /// <inheritdoc cref="QueryHistory.Solved"/>
        [Range(0, int.MaxValue)]
        public int Solved { get; set; }
    }
}
