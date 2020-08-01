using System.Threading.Tasks;
using FluentAssertions;
using OHunt.Web.Crawlers;
using OHunt.Web.Models;
using Xunit;

namespace OHunt.Tests.Crawlers
{
    public class UvaMappingCrawlersTests
    {
        [Fact]
        [Trait("Category", "WithNetwork")]
        public async Task Uva_ShouldWorkCorrectly()
        {
            // arrange
            var crawler = new UvaMappingCrawler();

            // act
            var res = await crawler.GetProblemLabel(4141);

            // assert
            crawler.OnlineJudge.Should().Be(MappingOnlineJudge.UVA);
            res.Should().Be("1395");
        }

        [Fact]
        [Trait("Category", "WithNetwork")]
        public async Task UvaLive_ShouldWorkCorrectly()
        {
            // arrange
            var crawler = new UvaLiveMappingCrawler();

            // act
            var res = await crawler.GetProblemLabel(4141);

            // assert
            crawler.OnlineJudge.Should().Be(MappingOnlineJudge.UVALive);
            res.Should().Be("6130");
        }
    }
}
