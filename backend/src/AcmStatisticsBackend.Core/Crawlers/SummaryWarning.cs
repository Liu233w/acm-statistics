using System.Collections.Generic;
using Abp.Domain.Values;

namespace AcmStatisticsBackend.Crawlers
{
    /// <summary>
    /// A warning in <see cref="QuerySummary"/>
    /// </summary>
    public class SummaryWarning : ValueObject
    {
        public SummaryWarning(string crawlerName, string content)
        {
            CrawlerName = crawlerName;
            Content = content;
        }

        /// <summary>
        /// The crawler the warning is about
        /// </summary>
        public string CrawlerName { get; }

        /// <summary>
        /// Warning content
        /// </summary>
        public string Content { get; }

        protected override IEnumerable<object> GetAtomicValues()
        {
            yield return CrawlerName;
            yield return Content;
        }
    }
}
