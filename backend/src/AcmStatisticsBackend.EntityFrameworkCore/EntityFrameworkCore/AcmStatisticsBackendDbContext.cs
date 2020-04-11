using System.Collections.Generic;
using Abp.Json;
using Abp.Zero.EntityFrameworkCore;
using AcmStatisticsBackend.Authorization.Roles;
using AcmStatisticsBackend.Authorization.Users;
using AcmStatisticsBackend.Crawlers;
using AcmStatisticsBackend.MultiTenancy;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace AcmStatisticsBackend.EntityFrameworkCore
{
    public class AcmStatisticsBackendDbContext : AbpZeroDbContext<Tenant, Role, User, AcmStatisticsBackendDbContext>
    {
        /* Define a DbSet for each entity of the application */

        public DbSet<DefaultQuery> DefaultQueries { get; set; }

        public DbSet<OjCrawler> OjCrawlers { get; set; }

        public DbSet<AcHistory> AcHistories { get; set; }

        public DbSet<AcWorkerHistory> AcWorkerHistories { get; set; }

        public AcmStatisticsBackendDbContext(DbContextOptions<AcmStatisticsBackendDbContext> options)
            : base(options)
        {
        }
    }
}
