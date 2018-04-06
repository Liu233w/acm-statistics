// <copyright file="AcmStatisticsAbpMigratorModule.cs" company="西北工业大学ACM开发组">
// Copyright (c) 西北工业大学ACM开发组. All rights reserved.
// </copyright>

namespace AcmStatisticsAbp.Migrator
{
    using Abp.Events.Bus;
    using Abp.Modules;
    using Abp.Reflection.Extensions;
    using AcmStatisticsAbp.Configuration;
    using AcmStatisticsAbp.EntityFrameworkCore;
    using AcmStatisticsAbp.Migrator.DependencyInjection;
    using Castle.MicroKernel.Registration;
    using Microsoft.Extensions.Configuration;

    [DependsOn(typeof(AcmStatisticsAbpEntityFrameworkModule))]
    public class AcmStatisticsAbpMigratorModule : AbpModule
    {
        private readonly IConfigurationRoot appConfiguration;

        public AcmStatisticsAbpMigratorModule(AcmStatisticsAbpEntityFrameworkModule abpProjectNameEntityFrameworkModule)
        {
            abpProjectNameEntityFrameworkModule.SkipDbSeed = true;

            this.appConfiguration = AppConfigurations.Get(
                typeof(AcmStatisticsAbpMigratorModule).GetAssembly().GetDirectoryPathOrNull());
        }

        public override void PreInitialize()
        {
            this.Configuration.DefaultNameOrConnectionString = this.appConfiguration.GetConnectionString(
                AcmStatisticsAbpConsts.ConnectionStringName);

            this.Configuration.BackgroundJobs.IsJobExecutionEnabled = false;
            this.Configuration.ReplaceService(
                typeof(IEventBus),
                () => this.IocManager.IocContainer.Register(
                    Component.For<IEventBus>().Instance(NullEventBus.Instance)));
        }

        public override void Initialize()
        {
            this.IocManager.RegisterAssemblyByConvention(typeof(AcmStatisticsAbpMigratorModule).GetAssembly());
            ServiceCollectionRegistrar.Register(this.IocManager);
        }
    }
}
