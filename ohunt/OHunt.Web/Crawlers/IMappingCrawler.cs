using System.Threading.Tasks;
using OHunt.Web.Models;

namespace OHunt.Web.Crawlers
{
    /// <summary>
    /// The crawler to map problems labels in online judge.
    ///
    /// An oj may have a problem label mapping, for example, the result from
    /// the api of uva does not have the problem label, but a problem id.
    /// The crawler can get the label from the id. They can be then saved to the
    /// database to support batch querying.
    /// </summary>
    public interface IMappingCrawler
    {
        /// <summary>
        /// The OJ
        /// </summary>
        MappingOnlineJudge OnlineJudge { get; }

        /// <summary>
        /// Get the problem label from problem id.
        /// </summary>
        /// <returns>
        /// the mapped problem label. If it is null, the problem id
        /// does not have a label.
        /// </returns>
        Task<string?> GetProblemLabel(long problemId);
    }
}
