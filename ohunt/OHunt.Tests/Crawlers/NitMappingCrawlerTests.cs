using System;
using System.Threading.Tasks;
using FluentAssertions;
using OHunt.Web.Crawlers;
using OHunt.Web.Models;
using Xunit;

namespace OHunt.Tests.Crawlers
{
    public class NitMappingCrawlerTests
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
            var crawler = new NitMappingCrawler();

            // act
            var res = await crawler.GetProblemLabel(4229);

            // assert
            res.Should().Be("HDU-4355");
        }

        [Fact]
        [Trait("Category", "WithNetwork")]
        public async Task WhenRequestingLocalJudge_ItShouldReturnNull()
        {
            // arrange
            var crawler = new NitMappingCrawler();

            // act
            var res = await crawler.GetProblemLabel(293);

            // assert
            res.Should().BeNull();
        }
    }
}
