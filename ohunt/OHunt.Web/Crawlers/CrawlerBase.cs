using System;
using System.IO;
using System.Text.Json;
using System.Threading;
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

        protected async Task<IDocument> GetDocument(
            Url url,
            CancellationToken cancellationToken)
        {
            var delta = DateTime.Now - _lastRequestTime;
            if (delta < RequestInterval)
            {
                await Task.Delay(delta, cancellationToken);
            }

            var response = await url.GetStreamAsync(cancellationToken: cancellationToken);
            var document = await Context.OpenAsync(
                req => req.Content(response),
                cancellationToken);
            _lastRequestTime = DateTime.Now;

            return document;
        }

        protected async Task<JsonDocument> GetJson(
            IFlurlRequest request,
            CancellationToken cancellationToken)
        {
            var delta = DateTime.Now - _lastRequestTime;
            if (delta < RequestInterval)
            {
                await Task.Delay(delta, cancellationToken);
            }

            var result = await request.GetStreamAsync(cancellationToken: cancellationToken);
            _lastRequestTime = DateTime.Now;

            return await JsonDocument.ParseAsync(result, cancellationToken: cancellationToken);
        }

        protected Task<JsonDocument> GetJson(
            Url url,
            CancellationToken cancellationToken)
        {
            return GetJson(new FlurlRequest(url), cancellationToken);
        }

        protected Task<JsonDocument> GetJson(
            string url,
            CancellationToken cancellationToken)
        {
            return GetJson(new Url(url), cancellationToken);
        }
    }
}
