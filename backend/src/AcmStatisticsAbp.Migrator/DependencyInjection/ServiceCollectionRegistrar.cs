// <copyright file="ServiceCollectionRegistrar.cs" company="西北工业大学ACM开发组">
// Copyright (c) 西北工业大学ACM开发组. All rights reserved.
// </copyright>

namespace AcmStatisticsAbp.Migrator.DependencyInjection
{
    using Abp.Dependency;
    using AcmStatisticsAbp.Identity;
    using Castle.Windsor.MsDependencyInjection;
    using Microsoft.Extensions.DependencyInjection;

    public static class ServiceCollectionRegistrar
    {
        public static void Register(IIocManager iocManager)
        {
            var services = new ServiceCollection();

            IdentityRegistrar.Register(services);

            WindsorRegistrationHelper.CreateServiceProvider(iocManager.IocContainer, services);
        }
    }
}
