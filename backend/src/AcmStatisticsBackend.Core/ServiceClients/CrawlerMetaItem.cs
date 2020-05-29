using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace AcmStatisticsBackend.ServiceClients
{
    public class CrawlerMetaItem
    {
        [Required]
        public string CrawlerName { get; set; }

        [Required]
        public string CrawlerTitle { get; set; }

        [AllowNull]
        public string CrawlerDescription { get; set; }

        [Required]
        public string Url { get; set; }

        public bool IsVirtualJudge { get; set; }
    }
}
