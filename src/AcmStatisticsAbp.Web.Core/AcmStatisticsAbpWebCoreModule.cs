// <copyright file="AcmStatisticsAbpWebCoreModule.cs" company="西北工业大学ACM开发组">
// Copyright (c) 西北工业大学ACM开发组. All rights reserved.
// </copyright>

namespace AcmStatisticsAbp
{
    using System;
    using System.Text;
    using Abp.AspNetCore;
    using Abp.AspNetCore.Configuration;
    using Abp.Modules;
    using Abp.Reflection.Extensions;
    using Abp.Zero.Configuration;
    using AcmStatisticsAbp.Authentication.JwtBearer;
    using AcmStatisticsAbp.Configuration;
    using AcmStatisticsAbp.EntityFrameworkCore;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;
    using Microsoft.IdentityModel.Tokens;

#if FEATURE_SIGNALR
    using Abp.Web.SignalR;
#elif FEATURE_SIGNALR_ASPNETCORE
    using Abp.AspNetCore.SignalR;
#endif

    [DependsOn(
         typeof(AcmStatisticsAbpApplicationModule),
         typeof(AcmStatisticsAbpEntityFrameworkModule),
         typeof(AbpAspNetCoreModule)
#pragma warning disable SA1001, SA1115, SA1113, SA1111, SA1009
#if FEATURE_SIGNALR
        ,typeof(AbpWebSignalRModule)
#elif FEATURE_SIGNALR_ASPNETCORE
        ,typeof(AbpAspNetCoreSignalRModule)
#endif
     )]
#pragma warning restore
    public class AcmStatisticsAbpWebCoreModule : AbpModule
    {
        private readonly IHostingEnvironment env;
        private readonly IConfigurationRoot appConfiguration;

        public AcmStatisticsAbpWebCoreModule(IHostingEnvironment env)
        {
            this.env = env;
            this.appConfiguration = env.GetAppConfiguration();
        }

        public override void PreInitialize()
        {
            this.Configuration.DefaultNameOrConnectionString = this.appConfiguration.GetConnectionString(
                AcmStatisticsAbpConsts.ConnectionStringName);

            // Use database for language management
            this.Configuration.Modules.Zero().LanguageManagement.EnableDbLocalization();

            this.Configuration.Modules.AbpAspNetCore()
                 .CreateControllersForAppServices(
                     typeof(AcmStatisticsAbpApplicationModule).GetAssembly());

            this.ConfigureTokenAuth();
        }

        private void ConfigureTokenAuth()
        {
            this.IocManager.Register<TokenAuthConfiguration>();
            var tokenAuthConfig = this.IocManager.Resolve<TokenAuthConfiguration>();

            tokenAuthConfig.SecurityKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(this.appConfiguration["Authentication:JwtBearer:SecurityKey"]));
            tokenAuthConfig.Issuer = this.appConfiguration["Authentication:JwtBearer:Issuer"];
            tokenAuthConfig.Audience = this.appConfiguration["Authentication:JwtBearer:Audience"];
            tokenAuthConfig.SigningCredentials = new SigningCredentials(tokenAuthConfig.SecurityKey, SecurityAlgorithms.HmacSha256);
            tokenAuthConfig.Expiration = TimeSpan.FromDays(1);
        }

        public override void Initialize()
        {
            this.IocManager.RegisterAssemblyByConvention(typeof(AcmStatisticsAbpWebCoreModule).GetAssembly());
        }
    }
}
