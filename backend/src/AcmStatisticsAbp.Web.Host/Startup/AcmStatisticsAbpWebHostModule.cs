// <copyright file="AcmStatisticsAbpWebHostModule.cs" company="西北工业大学ACM开发组">
// Copyright (c) 西北工业大学ACM开发组. All rights reserved.
// </copyright>

namespace AcmStatisticsAbp.Web.Host.Startup
{
    using Abp.Modules;
    using Abp.Reflection.Extensions;
    using AcmStatisticsAbp.Configuration;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;

    [DependsOn(
       typeof(AcmStatisticsAbpWebCoreModule))]
    public class AcmStatisticsAbpWebHostModule : AbpModule
    {
        private readonly IHostingEnvironment env;
        private readonly IConfigurationRoot appConfiguration;

        public AcmStatisticsAbpWebHostModule(IHostingEnvironment env)
        {
            this.env = env;
            this.appConfiguration = env.GetAppConfiguration();
        }

        public override void Initialize()
        {
            this.IocManager.RegisterAssemblyByConvention(typeof(AcmStatisticsAbpWebHostModule).GetAssembly());
        }
    }
}
