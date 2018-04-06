// <copyright file="AcmStatisticsAbpTestModule.cs" company="西北工业大学ACM开发组">
// Copyright (c) 西北工业大学ACM开发组. All rights reserved.
// </copyright>

namespace AcmStatisticsAbp.Tests
{
    using System;
    using Abp.AutoMapper;
    using Abp.Configuration.Startup;
    using Abp.Dependency;
    using Abp.Modules;
    using Abp.Net.Mail;
    using Abp.TestBase;
    using Abp.Zero.Configuration;
    using Abp.Zero.EntityFrameworkCore;
    using AcmStatisticsAbp.EntityFrameworkCore;
    using AcmStatisticsAbp.Tests.DependencyInjection;
    using Castle.MicroKernel.Registration;
    using NSubstitute;

    [DependsOn(
        typeof(AcmStatisticsAbpApplicationModule),
        typeof(AcmStatisticsAbpEntityFrameworkModule),
        typeof(AbpTestBaseModule))]
    public class AcmStatisticsAbpTestModule : AbpModule
    {
        public AcmStatisticsAbpTestModule(AcmStatisticsAbpEntityFrameworkModule abpProjectNameEntityFrameworkModule)
        {
            abpProjectNameEntityFrameworkModule.SkipDbContextRegistration = true;
            abpProjectNameEntityFrameworkModule.SkipDbSeed = true;
        }

        public override void PreInitialize()
        {
            this.Configuration.UnitOfWork.Timeout = TimeSpan.FromMinutes(30);
            this.Configuration.UnitOfWork.IsTransactional = false;

            // Disable static mapper usage since it breaks unit tests (see https://github.com/aspnetboilerplate/aspnetboilerplate/issues/2052)
            this.Configuration.Modules.AbpAutoMapper().UseStaticMapper = false;

            this.Configuration.BackgroundJobs.IsJobExecutionEnabled = false;

            // Use database for language management
            this.Configuration.Modules.Zero().LanguageManagement.EnableDbLocalization();

            this.RegisterFakeService<AbpZeroDbMigrator<AcmStatisticsAbpDbContext>>();

            this.Configuration.ReplaceService<IEmailSender, NullEmailSender>(DependencyLifeStyle.Transient);
        }

        public override void Initialize()
        {
            ServiceCollectionRegistrar.Register(this.IocManager);
        }

        private void RegisterFakeService<TService>()
            where TService : class
        {
            this.IocManager.IocContainer.Register(
                Component.For<TService>()
                    .UsingFactoryMethod(() => Substitute.For<TService>())
                    .LifestyleSingleton());
        }
    }
}
