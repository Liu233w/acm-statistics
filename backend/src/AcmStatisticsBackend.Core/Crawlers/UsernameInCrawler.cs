using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using Abp.Domain.Entities;

namespace AcmStatisticsBackend.Crawlers
{
    /// <summary>
    /// An username in a certain crawler.
    /// </summary>
    [DebuggerDisplay("FromCrawlerName = {FromCrawlerName}, Username = {Username}")]
    public class UsernameInCrawler : Entity<long>
    {
        /// <summary>
        /// The QueryCrawlerSummary it related to.
        /// </summary>
        [Required]
        public QueryCrawlerSummary QueryCrawlerSummary { get; set; }

        /// <summary>
        /// Which crawler (virtual judge) the username is from.
        ///
        /// If it is null or empty string, the username is from
        /// its own crawler.
        /// </summary>
        public string FromCrawlerName { get; set; }

        [Required]
        public string Username { get; set; }
    }
}
