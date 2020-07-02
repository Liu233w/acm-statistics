using System;
using System.Diagnostics;
using System.Net;
using System.Text.Json;
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
        private readonly ILogger<ZojSubmissionCrawler> _logger;

        public ZojSubmissionCrawler(ILogger<ZojSubmissionCrawler> logger)
        {
            _logger = logger;
            RequestInterval = TimeSpan.FromMilliseconds(100);
        }

        public OnlineJudge OnlineJudge => OnlineJudge.ZOJ;

        public async Task WorkAsync(long? lastSubmissionId, ITargetBlock<Submission> target)
        {
            try
            {
                await DoWork(lastSubmissionId, target);
            }
            catch (Exception e)
            {
                // error in crawling won't break existing records
                // so we can just save crawled results.
                _logger.LogError(e, "Error occured when crawling zoj");
                target.Complete();
            }
        }

        private async Task DoWork(long? lastSubmissionId, ITargetBlock<Submission> target)
        {
            var url = "https://zoj.pintia.cn/api/problem-sets/91827364500/submissions"
                .SetQueryParam("exam_id", "1278261888050192384")
                .SetQueryParam("show_all", "true");

            var id = lastSubmissionId ?? 1;

            JsonElement root;
            do
            {
                var request = url.SetQueryParam("after", id)
                    .WithHeader("Accept", "application/json;charset=UTF-8")
                    .AllowHttpStatus(HttpStatusCode.InternalServerError);
                _logger.LogTrace("Requesting url {0}", request.Url);

                var json = await GetJson(request);
                root = json.RootElement;

                id = 0;

                foreach (var submission in root.GetProperty("submissions").EnumerateArray())
                {
                    var idStr = await ParseOneSubmission(submission, target);
                    if (id == 0)
                    {
                        id = long.Parse(idStr);
                    }
                }
            } while (root.GetProperty("hasAfter").GetBoolean());

            target.Complete();
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
        /// <param name="target"></param>
        /// <returns>the new after to resume crawling</returns>
        private Task<long> TrySkipErrorSubmission(long oldAfter, ITargetBlock<Submission> target)
        {
            // use a binary search to quickly find the error submission
            long newAfter = oldAfter + 100;

            throw new NotImplementedException();
        }

        private async Task<string> ParseOneSubmission(JsonElement submission, ITargetBlock<Submission> target)
        {
            var idStr = submission.GetProperty("id").GetString();

            await target.SendAsync(new Submission
            {
                OnlineJudgeId = OnlineJudge,
                SubmissionId = long.Parse(idStr),
                UserName = submission.GetProperty("user")
                    .GetProperty("user")
                    .GetProperty("nickname")
                    .GetString(),
                Status = ParseStatus(submission.GetProperty("status").GetString()),
                ProblemLabel = submission.GetProperty("problemSetProblem")
                    .GetProperty("label")
                    .GetString(),
                Time = DateTime.Parse(submission.GetProperty("submitAt").GetString()),
            });

            return idStr;
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
    }
}
