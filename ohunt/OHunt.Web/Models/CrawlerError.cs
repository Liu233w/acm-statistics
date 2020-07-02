using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OHunt.Web.Models
{
    /// <summary>
    /// The error message generated when crawling
    /// </summary>
    [Table("CrawlerError")]
    public class CrawlerError
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public long Id { get; set; }

        /// <summary>
        /// The label to identify the crawler
        /// </summary>
        public string Crawler { get; set; }

        /// <summary>
        /// General message
        /// </summary>
        [Required]
        public string Message { get; set; }

        /// <summary>
        /// The time when the error is generated
        /// </summary>
        public DateTime Time { get; set; }

        /// <summary>
        /// The data of the message
        /// </summary>
        public string? Data { get; set; }
    }
}
