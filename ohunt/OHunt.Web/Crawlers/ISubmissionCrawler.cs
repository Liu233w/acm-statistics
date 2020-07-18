using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using OHunt.Web.Models;

namespace OHunt.Web.Crawlers
{
    /// <summary>
    /// Represent a crawler of submission
    /// </summary>
    public interface ISubmissionCrawler
    {
        /// <summary>
        /// The OJ
        /// </summary>
        OnlineJudge OnlineJudge { get; }

        /// <summary>
        /// A producer to produce submission info asynchronously 
        /// </summary>
        /// <param name="lastSubmissionId">the submission id of the last crawler result</param>
        /// <param name="pipeline">the pipeline to send results and errors</param>
        /// <param name="cancellationToken">The token to cancel crawling</param>
        Task WorkAsync(
            long? lastSubmissionId,
            ITargetBlock<CrawlerMessage> pipeline,
            CancellationToken cancellationToken);
    }
}
