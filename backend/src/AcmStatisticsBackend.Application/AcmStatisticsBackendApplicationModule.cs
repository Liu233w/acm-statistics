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

            // Scan the assembly for classes which inherit from AutoMapper.Profile
            Configuration.Modules.AbpAutoMapper().Configurators.Add(
                cfg => cfg.AddMaps(thisAssembly));
        }
    }
}
