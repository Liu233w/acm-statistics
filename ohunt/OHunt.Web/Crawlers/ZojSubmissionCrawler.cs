using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using Flurl;
using Flurl.Http;
using Microsoft.Extensions.Logging;
using OHunt.Web.Models;
using OHunt.Web.Utils;

namespace OHunt.Web.Crawlers
{
    public class ZojSubmissionCrawler : CrawlerBase, ISubmissionCrawler
    {
        private static Url BaseUrl => "https://zoj.pintia.cn/api/problem-sets/91827364500/submissions"
            .SetQueryParam("show_all", "true");

        private readonly ILogger<ZojSubmissionCrawler> _logger;

        public ZojSubmissionCrawler(ILogger<ZojSubmissionCrawler> logger)
        {
            _logger = logger;
            RequestInterval = TimeSpan.FromMilliseconds(100);
        }

        public OnlineJudge OnlineJudge => OnlineJudge.ZOJ;

        public async Task WorkAsync(
            long? lastSubmissionId,
            ITargetBlock<CrawlerMessage> pipeline,
            CancellationToken cancellationToken)
        {
            var after = lastSubmissionId ?? 1;

            while (true)
            {
                using var json = await Request(after, cancellationToken);
                var root = json.RootElement;

                if (root.TryGetProperty("error", out _))
                {
                    after = await TrySkipErrorSubmission(after, pipeline, cancellationToken);
                }
                else
                {
                    after = await ParseSubmissions(
                        root.GetProperty("submissions").EnumerateArray(),
                        pipeline);
                    if (!root.GetProperty("hasAfter").GetBoolean())
                    {
                        break;
                    }
                }
            }
        }

        private async Task<JsonDocument> Request(
            long after,
            CancellationToken cancellationToken,
            long? before = null)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var request = BaseUrl.SetQueryParam("after", after)
                .WithHeader("Accept", "application/json;charset=UTF-8")
                .AllowHttpStatus(500);
            if (before != null)
            {
                request = request.SetQueryParam("before", before);
            }

            _logger.LogTrace("Requesting url {0}", request.Url);

            return await GetJson(request, cancellationToken);
        }

        /// <summary>
        /// Some page may show error message like below:
        /// <example>
        /// {"error":{"code":"UNKNOWN","message":"Unknown Error"}}
        /// </example>
        /// 
        /// Try to skip certain submission that shows the error.
        /// </summary>
        /// <param name="oldAfter">the after parameter that shows the error</param>
        /// <param name="pipeline"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>the new after to resume crawling</returns>
        private async Task<long> TrySkipErrorSubmission(
            long oldAfter,
            ITargetBlock<CrawlerMessage> pipeline,
            CancellationToken cancellationToken)
        {
            // use a binary search to quickly find the error submission
            _logger.LogTrace("Try to skip error record");

            // [50]  50 => 25 [25] 50 => [25] 50 => [12] 13 50

            long after = oldAfter;
            int length = 100;
            while (length > 1 && after <= oldAfter + 100)
            {
                // it request item between (after, before)
                // items that have id equal to `after` or `before` are excluded
                using var json = await Request(after, cancellationToken, after + length);
                var root = json.RootElement;

                if (root.TryGetProperty("error", out _))
                {
                    length /= 2;
                }
                else
                {
                    after = await ParseSubmissions(
                        root.GetProperty("submissions").EnumerateArray(), pipeline);
                }
            }

            var newAfter = after + length;
            _logger.LogWarning("Submissions that has id between {0} and {1} (endpoint included) are skipped",
                after + 1, newAfter);
            return newAfter;
        }

        /// <summary>
        /// Parse the json array of submission, return the biggest id number
        /// </summary>
        private async Task<long> ParseSubmissions(
            JsonElement.ArrayEnumerator array,
            ITargetBlock<CrawlerMessage> pipeline)
        {
            long maxId = 0;
            foreach (var submission in array)
            {
                if (submission.GetProperty("problemType").GetString() != "PROGRAMMING")
                {
                    await pipeline.SendAsync(ErrorMessage(
                        "problemType is not PROGRAMMING",
                        submission.GetRawText()));
                    continue;
                }

                var idStr = submission.GetProperty("id").GetString();
                var id = long.Parse(idStr);
                maxId = Math.Max(id, maxId);

                try
                {
                    await pipeline.SendAsync(new CrawlerMessage
                    {
                        Submission = new Submission
                        {
                            OnlineJudgeId = OnlineJudge,
                            SubmissionId = id,
                            UserName = submission.GetProperty("user")
                                .GetProperty("user")
                                .GetProperty("nickname")
                                .GetString(),
                            Status = ParseStatus(submission.GetProperty("status").GetString()),
                            ProblemLabel = submission.GetProperty("problemSetProblem")
                                .GetProperty("label")
                                .GetString(),
                            Time = DateTime.Parse(submission.GetProperty("submitAt").GetString()),
                        },
                        Checkpoint = true,
                    });
                }
                catch (KeyNotFoundException e)
                {
                    await pipeline.SendAsync(ErrorMessage(
                        e.Message, submission.GetRawText()));
                }
            }

            return maxId;
        }

        private static RunResult ParseStatus(string status)
        {
            // from TIME_LIMIT_EXCEPTION
            var label = status.Split("_")
                // => TIME LIMIT EXCEPTION
                .JoinToString();
            // to TIMELIMITEXCEPTION

            if (Enum.TryParse<RunResult>(label, true, out var e))
            {
                return e;
            }
            else
            {
                return RunResult.UnknownError;
            }
        }

        private static CrawlerMessage ErrorMessage(string message, string data)
        {
            return new CrawlerMessage
            {
                CrawlerError = new CrawlerError
                {
                    Crawler = nameof(ZojSubmissionCrawler),
                    Message = message,
                    Time = DateTime.Now,
                    Data = data,
                },
                Checkpoint = true,
            };
        }
    }
}
