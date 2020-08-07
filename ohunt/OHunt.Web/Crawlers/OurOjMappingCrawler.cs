using System.Threading;
using System.Threading.Tasks;
using AngleSharp.Dom;
using Flurl;
using OHunt.Web.Models;

namespace OHunt.Web.Crawlers
{
    public class OurOjMappingCrawler : CrawlerBase, IMappingCrawler
    {
        public MappingOnlineJudge OnlineJudge => MappingOnlineJudge.OurOJ;

        public async Task<string?> GetProblemLabel(long problemId)
        {
            using var document = await GetDocument(
                "https://www.nitacm.com/problem_show.php"
                    .SetQueryParam("pid", problemId),
                CancellationToken.None);

            var ojDom = document.Body
                .QuerySelector("span.badge.badge-info");

            if (ojDom == null)
            {
                return null;
            }

            var oj = ojDom.Text();
            var label = ojDom.NextElementSibling.Text();

            return $"{oj}-{label}";
        }
    }
}
