// <copyright file="SeedHelper.cs" company="西北工业大学ACM开发组">
// Copyright (c) 西北工业大学ACM开发组. All rights reserved.
// </copyright>

namespace AcmStatisticsAbp.EntityFrameworkCore.Seed
{
    using System;
    using System.Transactions;
    using Abp.Dependency;
    using Abp.Domain.Uow;
    using Abp.EntityFrameworkCore.Uow;
    using Abp.MultiTenancy;
    using AcmStatisticsAbp.EntityFrameworkCore.Seed.Host;
    using AcmStatisticsAbp.EntityFrameworkCore.Seed.Tenants;
    using Microsoft.EntityFrameworkCore;

    public static class SeedHelper
    {
        public static void SeedHostDb(IIocResolver iocResolver)
        {
            WithDbContext<AcmStatisticsAbpDbContext>(iocResolver, SeedHostDb);
        }

        public static void SeedHostDb(AcmStatisticsAbpDbContext context)
        {
            context.SuppressAutoSetTenantId = true;

            // Host seed
            new InitialHostDbBuilder(context).Create();

            // Default tenant seed (in host database).
            new DefaultTenantBuilder(context).Create();
            new TenantRoleAndUserBuilder(context, 1).Create();
        }

        private static void WithDbContext<TDbContext>(IIocResolver iocResolver, Action<TDbContext> contextAction)
            where TDbContext : DbContext
        {
            using (var uowManager = iocResolver.ResolveAsDisposable<IUnitOfWorkManager>())
            {
                using (var uow = uowManager.Object.Begin(TransactionScopeOption.Suppress))
                {
                    var context = uowManager.Object.Current.GetDbContext<TDbContext>(MultiTenancySides.Host);

                    contextAction(context);

                    uow.Complete();
                }
            }
        }
    }
}
