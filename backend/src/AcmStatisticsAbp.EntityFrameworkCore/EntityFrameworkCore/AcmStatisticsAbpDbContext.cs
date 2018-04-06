// <copyright file="AcmStatisticsAbpDbContext.cs" company="西北工业大学ACM开发组">
// Copyright (c) 西北工业大学ACM开发组. All rights reserved.
// </copyright>

namespace AcmStatisticsAbp.EntityFrameworkCore
{
    using Abp.Zero.EntityFrameworkCore;
    using AcmStatisticsAbp.Authorization.Roles;
    using AcmStatisticsAbp.Authorization.Users;
    using AcmStatisticsAbp.MultiTenancy;
    using Microsoft.EntityFrameworkCore;

    public class AcmStatisticsAbpDbContext : AbpZeroDbContext<Tenant, Role, User, AcmStatisticsAbpDbContext>
    {
        /* Define a DbSet for each entity of the application */

        public AcmStatisticsAbpDbContext(DbContextOptions<AcmStatisticsAbpDbContext> options)
            : base(options)
        {
        }
    }
}
