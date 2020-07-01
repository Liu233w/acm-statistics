using System.Net.Http;
using Flurl.Http.Configuration;

namespace OHunt.Web.Utils
{
    public class PollyHttpClientFactory : DefaultHttpClientFactory
    {
        public override HttpMessageHandler CreateMessageHandler()
        {
            return new HttpClientPolicyHandler
            {
                InnerHandler = base.CreateMessageHandler(),
            };
        }
    }
}
