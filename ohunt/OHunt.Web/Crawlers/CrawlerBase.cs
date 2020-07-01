using System;
using System.Text.Json;
using System.Threading.Tasks;
using AngleSharp;
using AngleSharp.Dom;
using Flurl.Http;
using Url = Flurl.Url;

namespace OHunt.Web.Crawlers
{
    public abstract class CrawlerBase
    {
        private DateTime _lastRequestTime = DateTime.MinValue;

        protected TimeSpan RequestInterval { get; set; } = TimeSpan.FromMilliseconds(500);

        protected IBrowsingContext Context { get; }
            = BrowsingContext.New(Configuration.Default);

        protected async Task<IDocument> GetDocument(Url url)
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

        protected async Task<JsonDocument> GetJson(Url url)
        {
            var delta = DateTime.Now - _lastRequestTime;
            if (delta < RequestInterval)
            {
                await Task.Delay(delta);
            }

            var result = await url.GetStreamAsync();
            _lastRequestTime = DateTime.Now;
            return await JsonDocument.ParseAsync(result);
        }
    }
}
