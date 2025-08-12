using System.Runtime.CompilerServices;
using Flurl.Http;

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
            FlurlHttp.ConfigureClientForUrl("https://icpcarchive.ecs.baylor.edu/uhunt")
                .ConfigureInnerHandler(handler => handler.ServerCertificateCustomValidationCallback = (msg, cert, chain, errors) => true);
        }
    }
}
