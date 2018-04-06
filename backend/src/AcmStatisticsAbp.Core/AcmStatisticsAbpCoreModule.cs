// <copyright file="AcmStatisticsAbpCoreModule.cs" company="西北工业大学ACM开发组">
// Copyright (c) 西北工业大学ACM开发组. All rights reserved.
// </copyright>

namespace AcmStatisticsAbp
{
    using Abp.Modules;
    using Abp.Reflection.Extensions;
    using Abp.Timing;
    using Abp.Zero;
    using Abp.Zero.Configuration;
    using AcmStatisticsAbp.Authorization.Roles;
    using AcmStatisticsAbp.Authorization.Users;
    using AcmStatisticsAbp.Configuration;
    using AcmStatisticsAbp.Localization;
    using AcmStatisticsAbp.MultiTenancy;
    using AcmStatisticsAbp.Timing;

    [DependsOn(typeof(AbpZeroCoreModule))]
    public class AcmStatisticsAbpCoreModule : AbpModule
    {
        public override void PreInitialize()
        {
            this.Configuration.Auditing.IsEnabledForAnonymousUsers = true;

            // Declare entity types
            this.Configuration.Modules.Zero().EntityTypes.Tenant = typeof(Tenant);
            this.Configuration.Modules.Zero().EntityTypes.Role = typeof(Role);
            this.Configuration.Modules.Zero().EntityTypes.User = typeof(User);

            AcmStatisticsAbpLocalizationConfigurer.Configure(this.Configuration.Localization);

            // Enable this line to create a multi-tenant application.
            this.Configuration.MultiTenancy.IsEnabled = AcmStatisticsAbpConsts.MultiTenancyEnabled;

            // Configure roles
            AppRoleConfig.Configure(this.Configuration.Modules.Zero().RoleManagement);

            this.Configuration.Settings.Providers.Add<AppSettingProvider>();
        }

        public override void Initialize()
        {
            this.IocManager.RegisterAssemblyByConvention(typeof(AcmStatisticsAbpCoreModule).GetAssembly());
        }

        public override void PostInitialize()
        {
            this.IocManager.Resolve<AppTimes>().StartupTime = Clock.Now;
        }
    }
}
