using System.ComponentModel.DataAnnotations;
using AutoMapper;

namespace AcmStatisticsBackend.Crawlers.Dto
{
    [AutoMap(typeof(UsernameInCrawler))]
    public class UsernameInCrawlerDto
    {
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
