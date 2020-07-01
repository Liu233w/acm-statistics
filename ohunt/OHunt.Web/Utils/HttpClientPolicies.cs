using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using Polly;
using Polly.Retry;
using Polly.Timeout;
using Polly.Wrap;

namespace OHunt.Web.Utils
{
    public class HttpClientPolicies
    {
        public static AsyncPolicyWrap<HttpResponseMessage> PolicyStrategy =>
            Policy.WrapAsync(RetryPolicy);

        private static AsyncRetryPolicy<HttpResponseMessage> RetryPolicy =>
            Policy
                .HandleResult<HttpResponseMessage>(r => !r.IsSuccessStatusCode)
                .Or<TimeoutRejectedException>()
                .WaitAndRetryAsync(RetryIntervals(),
                    (delegateResult, retryCount) =>
                    {
                        Debug.WriteLine("[App|Policy]: Retrying http client request, attempt {0}, on url {1}",
                            retryCount, delegateResult.Result.RequestMessage.RequestUri);
                    });

        private static IEnumerable<TimeSpan> RetryIntervals()
        {
            var basicInterval = TimeSpan.FromSeconds(1);
            while (true)
            {
                yield return basicInterval;
                basicInterval.Multiply(2);
                if (basicInterval > TimeSpan.FromMinutes(10))
                {
                    yield break;
                }
            }
        }
    }
}
