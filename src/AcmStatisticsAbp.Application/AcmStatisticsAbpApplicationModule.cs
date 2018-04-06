// <copyright file="AcmStatisticsAbpApplicationModule.cs" company="西北工业大学ACM开发组">
// Copyright (c) 西北工业大学ACM开发组. All rights reserved.
// </copyright>

namespace AcmStatisticsAbp
{
    using Abp.AutoMapper;
    using Abp.Modules;
    using Abp.Reflection.Extensions;
    using AcmStatisticsAbp.Authorization;

    [DependsOn(
        typeof(AcmStatisticsAbpCoreModule),
        typeof(AbpAutoMapperModule))]
    public class AcmStatisticsAbpApplicationModule : AbpModule
    {
        public override void PreInitialize()
        {
            this.Configuration.Authorization.Providers.Add<AcmStatisticsAbpAuthorizationProvider>();
        }

        public override void Initialize()
        {
            var thisAssembly = typeof(AcmStatisticsAbpApplicationModule).GetAssembly();

            this.IocManager.RegisterAssemblyByConvention(thisAssembly);

            // Scan the assembly for classes which inherit from AutoMapper.Profile
            this.Configuration.Modules.AbpAutoMapper().Configurators.Add(
                cfg => cfg.AddProfiles(thisAssembly));
        }
    }
}
