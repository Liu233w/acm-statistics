using Abp.AutoMapper;
using Abp.Modules;
using Abp.Reflection.Extensions;
using AcmStatisticsBackend.Authorization;

namespace AcmStatisticsBackend
{
    [DependsOn(
        typeof(AcmStatisticsBackendCoreModule), 
        typeof(AbpAutoMapperModule))]
    public class AcmStatisticsBackendApplicationModule : AbpModule
    {
        public override void PreInitialize()
        {
            Configuration.Authorization.Providers.Add<AcmStatisticsBackendAuthorizationProvider>();
        }

        public override void Initialize()
        {
            var thisAssembly = typeof(AcmStatisticsBackendApplicationModule).GetAssembly();

            IocManager.RegisterAssemblyByConvention(thisAssembly);

            Configuration.Modules.AbpAutoMapper().Configurators.Add(
                // Scan the assembly for classes which inherit from AutoMapper.Profile
                cfg => cfg.AddMaps(thisAssembly)
            );
        }
    }
}
