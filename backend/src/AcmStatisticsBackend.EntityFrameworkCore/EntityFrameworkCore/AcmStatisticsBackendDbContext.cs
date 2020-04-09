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

        public AcmStatisticsBackendDbContext(DbContextOptions<AcmStatisticsBackendDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<DefaultQuery>()
                .Property(e => e.UsernamesInCrawlers)
                .HasConversion(
                    v => JsonConvert.SerializeObject(v),
                    v => JsonConvert.DeserializeObject<Dictionary<string, List<string>>>(v));
        }
    }
}
