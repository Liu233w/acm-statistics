using Abp.Events.Bus;
using Abp.Modules;
using Abp.Reflection.Extensions;
using AcmStatisticsBackend.Configuration;
using AcmStatisticsBackend.EntityFrameworkCore;
using AcmStatisticsBackend.Migrator.DependencyInjection;
using Castle.MicroKernel.Registration;
using Microsoft.Extensions.Configuration;

namespace AcmStatisticsBackend.Migrator
{
    [DependsOn(typeof(AcmStatisticsBackendEntityFrameworkModule))]
    public class AcmStatisticsBackendMigratorModule : AbpModule
    {
        private readonly IConfigurationRoot _appConfiguration;

        public AcmStatisticsBackendMigratorModule(AcmStatisticsBackendEntityFrameworkModule abpProjectNameEntityFrameworkModule)
        {
            abpProjectNameEntityFrameworkModule.SkipDbSeed = true;

            _appConfiguration = AppConfigurations.Get(
                typeof(AcmStatisticsBackendMigratorModule).GetAssembly().GetDirectoryPathOrNull());
        }

        public override void PreInitialize()
        {
            Configuration.DefaultNameOrConnectionString = _appConfiguration.GetConnectionString(
                AcmStatisticsBackendConsts.ConnectionStringName);

            Configuration.BackgroundJobs.IsJobExecutionEnabled = false;
            Configuration.ReplaceService(
                typeof(IEventBus),
                () => IocManager.IocContainer.Register(
                    Component.For<IEventBus>().Instance(NullEventBus.Instance)));
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(AcmStatisticsBackendMigratorModule).GetAssembly());
            ServiceCollectionRegistrar.Register(IocManager);
        }
    }
}
