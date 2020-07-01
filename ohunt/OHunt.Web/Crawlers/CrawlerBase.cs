using System;
using System.Threading.Tasks;
using AngleSharp;
using AngleSharp.Dom;

namespace OHunt.Web.Crawlers
{
    public abstract class CrawlerBase
    {
        private DateTime _lastRequestTime = DateTime.MinValue;

        protected TimeSpan RequestInterval = TimeSpan.FromMilliseconds(500);

        protected IBrowsingContext Context { get; }
            = BrowsingContext.New(Configuration.Default);

        protected async Task<IDocument> GetDocument(string url)
        {
            var delta = DateTime.Now - _lastRequestTime;
            if (delta < RequestInterval)
            {
                await Task.Delay(delta);
            }

            var document = await Context.OpenAsync(url);
            _lastRequestTime = DateTime.Now;

            return document;
        }
    }
}
