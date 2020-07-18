using OHunt.Web.Models;

namespace OHunt.Web.Crawlers
{
    /// <summary>
    /// Represent a message that a crawler can reproduce.
    /// </summary>
    public struct CrawlerMessage
    {
        /// <summary>
        /// The message carries a submission
        /// </summary>
        public Submission? Submission { get; set; }

        /// <summary>
        /// The message carries a crawler error
        /// </summary>
        public CrawlerError? CrawlerError { get; set; }

        /// <summary>
        /// The message creates a checkpoint.
        ///
        /// When pipeline is terminated (cancelled or exception),
        /// data before a checkpoint are saved.
        /// </summary>
        public bool IsCheckPoint { get; set; }
    }
}
