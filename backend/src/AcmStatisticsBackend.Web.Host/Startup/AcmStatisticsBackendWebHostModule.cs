using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Abp.Modules;
using Abp.Reflection.Extensions;
using AcmStatisticsBackend.Configuration;

namespace AcmStatisticsBackend.Web.Host.Startup
{
    [DependsOn(
       typeof(AcmStatisticsBackendWebCoreModule))]
    public class AcmStatisticsBackendWebHostModule: AbpModule
    {
        private readonly IWebHostEnvironment _env;
        private readonly IConfigurationRoot _appConfiguration;

        public AcmStatisticsBackendWebHostModule(IWebHostEnvironment env)
        {
            _env = env;
            _appConfiguration = env.GetAppConfiguration();
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(AcmStatisticsBackendWebHostModule).GetAssembly());
        }
    }
}
