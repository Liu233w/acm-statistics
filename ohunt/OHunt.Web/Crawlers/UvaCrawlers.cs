using System;
using System.Threading;
using System.Threading.Tasks;
using OHunt.Web.Models;

namespace OHunt.Web.Crawlers
{
    public abstract class UvaMappingCrawlerBase : CrawlerBase, IMappingCrawler
    {
        private readonly string _baseUrl;

        public UvaMappingCrawlerBase(string baseUrl)
        {
            _baseUrl = baseUrl;
            RequestInterval = TimeSpan.FromMilliseconds(50);
        }

        public abstract MappingOnlineJudge OnlineJudge { get; }

        public async Task<string?> GetProblemLabel(long problemId)
        {
            // TODO: make it cancelable
            using var document = await GetJson(
                _baseUrl + "/api/p/id/" + problemId, CancellationToken.None);
            return document.RootElement.GetProperty("num").GetInt64().ToString();
        }
    }

    public class UvaMappingCrawler : UvaMappingCrawlerBase
    {
        public UvaMappingCrawler() : base("https://uhunt.onlinejudge.org")
        {
        }

        public override MappingOnlineJudge OnlineJudge => MappingOnlineJudge.UVA;
    }

    public class UvaLiveMappingCrawler : UvaMappingCrawlerBase
    {
        public UvaLiveMappingCrawler() : base("https://icpcarchive.ecs.baylor.edu/uhunt")
        {
        }

        public override MappingOnlineJudge OnlineJudge => MappingOnlineJudge.UVALive;
    }
}
