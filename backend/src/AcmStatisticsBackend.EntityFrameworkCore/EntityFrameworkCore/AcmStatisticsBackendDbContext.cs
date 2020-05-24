using System.Collections.Generic;
using Abp.Json;
using Abp.Zero.EntityFrameworkCore;
using AcmStatisticsBackend.Authorization.Roles;
using AcmStatisticsBackend.Authorization.Users;
using AcmStatisticsBackend.Crawlers;
using AcmStatisticsBackend.MultiTenancy;
using AcmStatisticsBackend.Settings;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace AcmStatisticsBackend.EntityFrameworkCore
{
    public class AcmStatisticsBackendDbContext : AbpZeroDbContext<Tenant, Role, User, AcmStatisticsBackendDbContext>
    {
        /* Define a DbSet for each entity of the application */

        public DbSet<DefaultQuery> DefaultQueries { get; set; }

        public DbSet<QueryHistory> QueryHistories { get; set; }

        public DbSet<QueryWorkerHistory> QueryWorkerHistories { get; set; }

        public DbSet<UserSettingAttribute> UserSettingAttributes { get; set; }

        public DbSet<QuerySummary> QuerySummaries { get; set; }

        public DbSet<QueryCrawlerSummary> QueryCrawlerSummaries { get; set; }

        public DbSet<UsernameInCrawler> UsernameInCrawler { get; set; }

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

            modelBuilder.Entity<QueryWorkerHistory>()
                .Property(e => e.SolvedList)
                .HasConversion(v => JsonConvert.SerializeObject(v),
                    v => JsonConvert.DeserializeObject<string[]>(v));

            modelBuilder.Entity<QueryWorkerHistory>()
                .Property(e => e.SubmissionsByCrawlerName)
                .HasConversion(v => JsonConvert.SerializeObject(v),
                    v => JsonConvert.DeserializeObject<Dictionary<string, int>>(v));

            modelBuilder.Entity<QuerySummary>()
                .Property(e => e.SummaryWarnings)
                .HasConversion(v => JsonConvert.SerializeObject(v),
                    v => JsonConvert.DeserializeObject<List<string>>(v));
        }
    }
}
