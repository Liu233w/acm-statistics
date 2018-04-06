// <copyright file="MultiTenantMigrateExecuter.cs" company="西北工业大学ACM开发组">
// Copyright (c) 西北工业大学ACM开发组. All rights reserved.
// </copyright>

namespace AcmStatisticsAbp.Migrator
{
    using System;
    using System.Collections.Generic;
    using System.Data.Common;
    using Abp.Data;
    using Abp.Dependency;
    using Abp.Domain.Repositories;
    using Abp.Domain.Uow;
    using Abp.Extensions;
    using Abp.MultiTenancy;
    using Abp.Runtime.Security;
    using AcmStatisticsAbp.EntityFrameworkCore;
    using AcmStatisticsAbp.EntityFrameworkCore.Seed;
    using AcmStatisticsAbp.MultiTenancy;

    public class MultiTenantMigrateExecuter : ITransientDependency
    {
        private readonly Log log;
        private readonly AbpZeroDbMigrator migrator;
        private readonly IRepository<Tenant> tenantRepository;
        private readonly IDbPerTenantConnectionStringResolver connectionStringResolver;

        public MultiTenantMigrateExecuter(
            AbpZeroDbMigrator migrator,
            IRepository<Tenant> tenantRepository,
            Log log,
            IDbPerTenantConnectionStringResolver connectionStringResolver)
        {
            this.log = log;

            this.migrator = migrator;
            this.tenantRepository = tenantRepository;
            this.connectionStringResolver = connectionStringResolver;
        }

        public bool Run(bool skipConnVerification)
        {
            var hostConnStr = CensorConnectionString(this.connectionStringResolver.GetNameOrConnectionString(new ConnectionStringResolveArgs(MultiTenancySides.Host)));
            if (hostConnStr.IsNullOrWhiteSpace())
            {
                this.log.Write("Configuration file should contain a connection string named 'Default'");
                return false;
            }

            this.log.Write("Host database: " + ConnectionStringHelper.GetConnectionString(hostConnStr));
            if (!skipConnVerification)
            {
                this.log.Write("Continue to migration for this host database and all tenants..? (Y/N): ");
                var command = Console.ReadLine();
                if (!command.IsIn("Y", "y"))
                {
                    this.log.Write("Migration canceled.");
                    return false;
                }
            }

            this.log.Write("HOST database migration started...");

            try
            {
                this.migrator.CreateOrMigrateForHost(SeedHelper.SeedHostDb);
            }
            catch (Exception ex)
            {
                this.log.Write("An error occured during migration of host database:");
                this.log.Write(ex.ToString());
                this.log.Write("Canceled migrations.");
                return false;
            }

            this.log.Write("HOST database migration completed.");
            this.log.Write("--------------------------------------------------------");

            var migratedDatabases = new HashSet<string>();
            var tenants = this.tenantRepository.GetAllList(t => t.ConnectionString != null && t.ConnectionString != string.Empty);
            for (var i = 0; i < tenants.Count; i++)
            {
                var tenant = tenants[i];
                this.log.Write(string.Format("Tenant database migration started... ({0} / {1})", i + 1, tenants.Count));
                this.log.Write("Name              : " + tenant.Name);
                this.log.Write("TenancyName       : " + tenant.TenancyName);
                this.log.Write("Tenant Id         : " + tenant.Id);
                this.log.Write("Connection string : " + SimpleStringCipher.Instance.Decrypt(tenant.ConnectionString));

                if (!migratedDatabases.Contains(tenant.ConnectionString))
                {
                    try
                    {
                        this.migrator.CreateOrMigrateForTenant(tenant);
                    }
                    catch (Exception ex)
                    {
                        this.log.Write("An error occured during migration of tenant database:");
                        this.log.Write(ex.ToString());
                        this.log.Write("Skipped this tenant and will continue for others...");
                    }

                    migratedDatabases.Add(tenant.ConnectionString);
                }
                else
                {
                    this.log.Write("This database has already migrated before (you have more than one tenant in same database). Skipping it....");
                }

                this.log.Write(string.Format("Tenant database migration completed. ({0} / {1})", i + 1, tenants.Count));
                this.log.Write("--------------------------------------------------------");
            }

            this.log.Write("All databases have been migrated.");

            return true;
        }

        private static string CensorConnectionString(string connectionString)
        {
            var builder = new DbConnectionStringBuilder { ConnectionString = connectionString };
            var keysToMask = new[] { "password", "pwd", "user id", "uid" };

            foreach (var key in keysToMask)
            {
                if (builder.ContainsKey(key))
                {
                    builder[key] = "*****";
                }
            }

            return builder.ToString();
        }
    }
}
