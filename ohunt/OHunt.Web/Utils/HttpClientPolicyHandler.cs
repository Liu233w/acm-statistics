using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace OHunt.Web.Utils
{
    public class HttpClientPolicyHandler : DelegatingHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            return HttpClientPolicies.PolicyStrategy.ExecuteAsync(ct => base.SendAsync(request, ct), cancellationToken);
        }
    }
}
