using System;
using System.Diagnostics;
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

        public async Task Work(long? lastSubmissionId, ITargetBlock<Submission> target)
        {
            var url = "https://zoj.pintia.cn/api/problem-sets/91827364500/submissions"
                .SetQueryParam("exam_id", "1278261888050192384")
                .SetQueryParam("show_all", "true");

            var id = lastSubmissionId?.ToString() ?? "1";

            JsonElement root;
            do
            {
                var request = url.SetQueryParam("after", id)
                    .WithHeader("Accept", "application/json;charset=UTF-8");
                _logger.LogTrace("Requesting url {0}", request.Url);
                var json = await GetJson(request);
                root = json.RootElement;

                id = string.Empty;

                foreach (var submission in root.GetProperty("submissions").EnumerateArray())
                {
                    var idStr = submission.GetProperty("id").GetString();
                    if (id == string.Empty)
                    {
                        id = idStr;
                    }

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
                }
            } while (root.GetProperty("hasAfter").GetBoolean());

            target.Complete();
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
