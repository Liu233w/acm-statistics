using Abp.EntityFrameworkCore.Configuration;
using Abp.Modules;
using Abp.Reflection.Extensions;
using Abp.Zero.EntityFrameworkCore;
using AcmStatisticsBackend.EntityFrameworkCore.Seed;

namespace AcmStatisticsBackend.EntityFrameworkCore
{
    [DependsOn(
        typeof(AcmStatisticsBackendCoreModule), 
        typeof(AbpZeroCoreEntityFrameworkCoreModule))]
    public class AcmStatisticsBackendEntityFrameworkModule : AbpModule
    {
        /* Used it tests to skip dbcontext registration, in order to use in-memory database of EF Core */
        public bool SkipDbContextRegistration { get; set; }

        public bool SkipDbSeed { get; set; }

        public override void PreInitialize()
        {
            if (!SkipDbContextRegistration)
            {
                Configuration.Modules.AbpEfCore().AddDbContext<AcmStatisticsBackendDbContext>(options =>
                {
                    if (options.ExistingConnection != null)
                    {
                        AcmStatisticsBackendDbContextConfigurer.Configure(options.DbContextOptions, options.ExistingConnection);
                    }
                    else
                    {
                        AcmStatisticsBackendDbContextConfigurer.Configure(options.DbContextOptions, options.ConnectionString);
                    }
                });
            }
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(AcmStatisticsBackendEntityFrameworkModule).GetAssembly());
        }

        public override void PostInitialize()
        {
            if (!SkipDbSeed)
            {
                SeedHelper.SeedHostDb(IocManager);
            }
        }
    }
}
