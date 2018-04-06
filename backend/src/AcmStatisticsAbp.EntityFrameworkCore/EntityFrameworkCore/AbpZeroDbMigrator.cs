// <copyright file="AbpZeroDbMigrator.cs" company="西北工业大学ACM开发组">
// Copyright (c) 西北工业大学ACM开发组. All rights reserved.
// </copyright>

namespace AcmStatisticsAbp.EntityFrameworkCore
{
    using Abp.Domain.Uow;
    using Abp.EntityFrameworkCore;
    using Abp.MultiTenancy;
    using Abp.Zero.EntityFrameworkCore;

    public class AbpZeroDbMigrator : AbpZeroDbMigrator<AcmStatisticsAbpDbContext>
    {
        public AbpZeroDbMigrator(
            IUnitOfWorkManager unitOfWorkManager,
            IDbPerTenantConnectionStringResolver connectionStringResolver,
            IDbContextResolver dbContextResolver)
            : base(
                unitOfWorkManager,
                connectionStringResolver,
                dbContextResolver)
        {
        }
    }
}
