using System;
using System.Threading.Tasks;
using FluentAssertions;
using OHunt.Web.Crawlers;
using OHunt.Web.Models;
using Xunit;

namespace OHunt.Tests.Crawlers
{
    public class OurOjMappingCrawlerTests
    {
        [Fact]
        public void It_ShouldReturnCorrectEnum()
        {
            new OurOjMappingCrawler()
                .OnlineJudge
                .Should().Be(MappingOnlineJudge.OurOJ);
        }

        [Fact]
        [Trait("Category", "WithNetwork")]
        public async Task WhenRequestingVirtualJudge_ItShouldReturnOjAndLabel()
        {
            // arrange
            var crawler = new OurOjMappingCrawler();

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
            var crawler = new OurOjMappingCrawler();

            // act
            var res = await crawler.GetProblemLabel(293);

            // assert
            res.Should().BeNull();
        }
    }
}
