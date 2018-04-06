// <copyright file="ServiceCollectionRegistrar.cs" company="西北工业大学ACM开发组">
// Copyright (c) 西北工业大学ACM开发组. All rights reserved.
// </copyright>

namespace AcmStatisticsAbp.Tests.DependencyInjection
{
    using System;
    using Abp.Dependency;
    using AcmStatisticsAbp.EntityFrameworkCore;
    using AcmStatisticsAbp.Identity;
    using Castle.MicroKernel.Registration;
    using Castle.Windsor.MsDependencyInjection;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.DependencyInjection;

    public static class ServiceCollectionRegistrar
    {
        public static void Register(IIocManager iocManager)
        {
            var services = new ServiceCollection();

            IdentityRegistrar.Register(services);

            services.AddEntityFrameworkInMemoryDatabase();

            var serviceProvider = WindsorRegistrationHelper.CreateServiceProvider(iocManager.IocContainer, services);

            var builder = new DbContextOptionsBuilder<AcmStatisticsAbpDbContext>();
            builder.UseInMemoryDatabase(Guid.NewGuid().ToString()).UseInternalServiceProvider(serviceProvider);

            iocManager.IocContainer.Register(
                Component
                    .For<DbContextOptions<AcmStatisticsAbpDbContext>>()
                    .Instance(builder.Options)
                    .LifestyleSingleton());
        }
    }
}
