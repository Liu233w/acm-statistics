using Abp.Domain.Uow;
using Abp.EntityFrameworkCore;
using Abp.EntityFrameworkCore.Configuration;
using Abp.Modules;
using Abp.MultiTenancy;
using Abp.Reflection.Extensions;
using Abp.Zero.EntityFrameworkCore;
using AcmStatisticsBackend.EntityFrameworkCore.Seed;
using Microsoft.EntityFrameworkCore;

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
            if (!SkipDbContextRegistration)
            {
                var dbContextProvider = IocManager.Resolve<IDbContextProvider<AcmStatisticsBackendDbContext>>();
                var unitOfWorkManager = IocManager.Resolve<IUnitOfWorkManager>();

                using (var unitOfWork = unitOfWorkManager.Begin())
                {
                    var context = dbContextProvider.GetDbContext(MultiTenancySides.Host);
                    // Removes actual connection as it has been enlisted in a non needed transaction for migration
                    context.Database.CloseConnection();
                    context.Database.Migrate();
                }
            }

            if (!SkipDbSeed)
            {
                SeedHelper.SeedHostDb(IocManager);
            }
        }
    }
}
