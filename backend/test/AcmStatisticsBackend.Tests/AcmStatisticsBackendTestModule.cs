using System;
using Abp.AutoMapper;
using Abp.Configuration.Startup;
using Abp.Dependency;
using Abp.Modules;
using Abp.Net.Mail;
using Abp.TestBase;
using Abp.Zero.Configuration;
using Abp.Zero.EntityFrameworkCore;
using AcmStatisticsBackend.EntityFrameworkCore;
using AcmStatisticsBackend.Tests.DependencyInjection;
using Castle.MicroKernel.Registration;
using NSubstitute;
using Xunit;

namespace AcmStatisticsBackend.Tests
{
    [DependsOn(
        typeof(AcmStatisticsBackendApplicationModule),
        typeof(AcmStatisticsBackendEntityFrameworkModule),
        typeof(AbpTestBaseModule))]
    public class AcmStatisticsBackendTestModule : AbpModule
    {
        private readonly ServiceCollectionRegistrar _registrar;

        public AcmStatisticsBackendTestModule(
            AcmStatisticsBackendEntityFrameworkModule abpProjectNameEntityFrameworkModule)
        {
            abpProjectNameEntityFrameworkModule.SkipDbContextRegistration = true;
            abpProjectNameEntityFrameworkModule.SkipDbSeed = true;

            _registrar = new ServiceCollectionRegistrar();
        }

        public override void PreInitialize()
        {
            Configuration.UnitOfWork.Timeout = TimeSpan.FromMinutes(30);
            Configuration.UnitOfWork.IsTransactional = false;

            // Disable static mapper usage since it breaks unit tests (see https://github.com/aspnetboilerplate/aspnetboilerplate/issues/2052)
            Configuration.Modules.AbpAutoMapper().UseStaticMapper = false;

            Configuration.BackgroundJobs.IsJobExecutionEnabled = false;

            // Use database for language management
            Configuration.Modules.Zero().LanguageManagement.EnableDbLocalization();

            RegisterFakeService<AbpZeroDbMigrator<AcmStatisticsBackendDbContext>>();

            Configuration.ReplaceService<IEmailSender, NullEmailSender>(DependencyLifeStyle.Transient);
        }

        public override void Initialize()
        {
            _registrar.Register(IocManager);
        }

        public override void Shutdown()
        {
            _registrar.Dispose();
            base.Shutdown();
        }

        private void RegisterFakeService<TService>() where TService : class
        {
            IocManager.IocContainer.Register(
                Component.For<TService>()
                    .UsingFactoryMethod(() => Substitute.For<TService>())
                    .LifestyleSingleton());
        }
    }
}
