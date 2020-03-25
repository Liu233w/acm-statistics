using System;
using Abp.Dependency;

namespace AcmStatisticsBackend.Timing
{
    public class AppTimes : ISingletonDependency
    {
        /// <summary>
        /// Gets or sets the startup time of the application.
        /// </summary>
        public DateTime StartupTime { get; set; }
    }
}
