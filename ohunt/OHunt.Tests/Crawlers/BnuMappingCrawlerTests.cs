using System;
using System.Threading.Tasks;
using FluentAssertions;
using OHunt.Web.Crawlers;
using OHunt.Web.Models;
using Xunit;

namespace OHunt.Tests.Crawlers
{
    public class BnuMappingCrawlerTests
    {
        [Fact]
        public void It_ShouldReturnCorrectEnum()
        {
            new NitMappingCrawler()
                .OnlineJudge
                .Should().Be(MappingOnlineJudge.NIT);
        }

        [Fact]
        [Trait("Category", "WithNetwork")]
        public async Task WhenRequestingVirtualJudge_ItShouldReturnOjAndLabel()
        {
            // arrange
            var crawler = new BnuMappingCrawler();

            // act
            var res = await crawler.GetProblemLabel(32831);

            // assert
            res.Should().Be("HUST-1000");
        }

        [Fact]
        [Trait("Category", "WithNetwork")]
        public async Task WhenRequestingLocalJudge_ItShouldReturnNull()
        {
            // arrange
            var crawler = new BnuMappingCrawler();

            // act
            var res = await crawler.GetProblemLabel(1001);

            // assert
            res.Should().BeNull();
        }
    }
}
