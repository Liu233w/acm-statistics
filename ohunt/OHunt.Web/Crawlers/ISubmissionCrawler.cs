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
        /// <param name="target">to post the submissions</param>
        Task Work(long lastSubmissionId, ITargetBlock<Submission> target);
    }
}
