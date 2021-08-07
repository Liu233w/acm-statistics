using System.Net.Http;
using System.Net.Security;
using System.Runtime.CompilerServices;
using Flurl.Http;
using Flurl.Http.Configuration;

namespace OHunt.Web
{
    /// <summary>
    /// Setup some config that are applied for both runtime and testing
    /// </summary>
    public class GlobalConfigurer
    {
        [ModuleInitializer]
        public static void Configure()
        {
            FlurlHttp.ConfigureClient("https://icpcarchive.ecs.baylor.edu/uhunt", cli =>
                cli.Settings.HttpClientFactory = new UntrustedCertClientFactory());
        }
    }
    
    public class UntrustedCertClientFactory : DefaultHttpClientFactory
    {
        public override HttpMessageHandler CreateMessageHandler() {
            return new HttpClientHandler {
                ServerCertificateCustomValidationCallback = (a, b, c, d) => true
            };
        }
    }
}
