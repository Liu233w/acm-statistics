// <copyright file="AppTimes.cs" company="西北工业大学ACM开发组">
// Copyright (c) 西北工业大学ACM开发组. All rights reserved.
// </copyright>

namespace AcmStatisticsAbp.Timing
{
    using System;
    using Abp.Dependency;

    public class AppTimes : ISingletonDependency
    {
        /// <summary>
        /// Gets or sets the startup time of the application.
        /// </summary>
        public DateTime StartupTime { get; set; }
    }
}
