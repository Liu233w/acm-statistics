using System.Collections.Generic;
using System.Threading.Tasks;
using AcmStatisticsBackend.Crawlers;
using AcmStatisticsBackend.Crawlers.Dto;
using FluentAssertions;
using Xunit;

namespace AcmStatisticsBackend.Tests.Crawlers
{
    public class DefaultQueryAppService_Tests : AcmStatisticsBackendTestBase
    {
        private readonly IDefaultQueryAppService _appService;

        public DefaultQueryAppService_Tests()
        {
            _appService = Resolve<DefaultQueryAppService>();
        }

        [Fact]
        public async Task 能够正确获取存储的记录()
        {
            // act
            await _appService.SetDefaultQueries(new DefaultQueryDto
            {
                MainUsername = "mainUsername",
                UsernamesInCrawlers = new Dictionary<string, List<string>>
                {
                    { "crawler1", new List<string> { "username1", "username2" } },
                    { "crawler2", new List<string> { "username3", "username4" } },
                },
            });

            var result = await _appService.GetDefaultQueries();

            // assert
            result.Should().BeEquivalentTo(new DefaultQueryDto
            {
                MainUsername = "mainUsername",
                UsernamesInCrawlers = new Dictionary<string, List<string>>
                {
                    { "crawler1", new List<string> { "username1", "username2" } },
                    { "crawler2", new List<string> { "username3", "username4" } },
                },
            });
        }

        [Fact]
        public async Task 能存储空的主用户名()
        {
            // act
            await _appService.SetDefaultQueries(new DefaultQueryDto
            {
                MainUsername = "",
            });

            var result = await _appService.GetDefaultQueries();

            // assert
            result.Should().BeEquivalentTo(new DefaultQueryDto
            {
                MainUsername = "",
                UsernamesInCrawlers = new Dictionary<string, List<string>>(),
            });
        }

        [Fact]
        public async Task 能去除空的爬虫用户名()
        {
            // act
            await _appService.SetDefaultQueries(new DefaultQueryDto
            {
                MainUsername = "",
                UsernamesInCrawlers = new Dictionary<string, List<string>>
                {
                    { "crawler1", new List<string> { "username1", "username2" } },
                    { "crawler2", new List<string> { "" } },
                    { "crawler3", new List<string>() },
                    { "crawler4", new List<string> { "username1", "" } },
                    { "crawler5", new List<string> { "username1", null } },
                },
            });

            var result = await _appService.GetDefaultQueries();

            // assert
            result.Should().BeEquivalentTo(new DefaultQueryDto
            {
                MainUsername = "",
                UsernamesInCrawlers = new Dictionary<string, List<string>>
                {
                    { "crawler1", new List<string> { "username1", "username2" } },
                    { "crawler4", new List<string> { "username1" } },
                    { "crawler5", new List<string> { "username1" } },
                },
            });
        }

        [Fact]
        public async Task 没有记录时返回一个空结果()
        {
            // act
            var result = await _appService.GetDefaultQueries();

            // assert
            result.Should().BeEquivalentTo(new DefaultQueryDto
            {
                MainUsername = "",
                UsernamesInCrawlers = new Dictionary<string, List<string>>(),
            });
        }
    }
}
