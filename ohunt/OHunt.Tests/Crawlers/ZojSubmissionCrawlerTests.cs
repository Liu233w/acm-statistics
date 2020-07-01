using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using OHunt.Web.Crawlers;
using OHunt.Web.Models;
using Snapshooter.Xunit;
using Xunit;

namespace OHunt.Tests.Crawlers
{
    public class ZojSubmissionCrawlerTests
    {
        [Fact]
        public async Task It_ShouldGetCorrectResult()
        {
            var crawler = new ZojSubmissionCrawler();

            var submissionBuffer
                = new BufferBlock<Submission>(new DataflowBlockOptions
                {
                    EnsureOrdered = true,
                });

#pragma warning disable 4014
            crawler.Work(null, submissionBuffer);
#pragma warning restore 4014

            const int bufferLength = 110;

            var list = new Submission[bufferLength];
            for (var i = 0; i < bufferLength; i++)
            {
                list[i] = await submissionBuffer.ReceiveAsync();
            }

            Snapshot.Match(list);
        }
    }
}
