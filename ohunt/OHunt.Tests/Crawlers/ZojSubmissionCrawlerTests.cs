using System;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using OHunt.Web.Crawlers;
using OHunt.Web.Models;
using Snapshooter.Xunit;
using Xunit;

namespace OHunt.Tests.Crawlers
{
    [Trait("Category", "WithNetwork")]
    public class ZojSubmissionCrawlerTests
    {
        private readonly ILogger<ZojSubmissionCrawler> _loggerMock =
            Mock.Of<ILogger<ZojSubmissionCrawler>>();

        [Fact]
        public async Task It_ShouldGetCorrectResult()
        {
            var crawler = new ZojSubmissionCrawler(_loggerMock);

            var messages
                = new BufferBlock<CrawlerMessage>(new DataflowBlockOptions
                {
                    EnsureOrdered = true,
                });

#pragma warning disable 4014
            crawler.WorkAsync(null, messages, new CancellationToken());
#pragma warning restore 4014

            const int bufferLength = 110;

            var list = new Submission?[bufferLength];
            for (var i = 0; i < bufferLength; i++)
            {
                var message = await messages.ReceiveAsync(TimeSpan.FromSeconds(30));
                message.CrawlerError.Should().BeNull();
                message.Checkpoint.Should().BeTrue();
                list[i] = message.Submission;
            }

            Snapshot.Match(list);
        }
    }
}
