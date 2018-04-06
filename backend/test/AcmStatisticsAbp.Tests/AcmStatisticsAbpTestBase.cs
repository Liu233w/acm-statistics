// <copyright file="AcmStatisticsAbpTestBase.cs" company="西北工业大学ACM开发组">
// Copyright (c) 西北工业大学ACM开发组. All rights reserved.
// </copyright>

namespace AcmStatisticsAbp.Tests
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Abp;
    using Abp.Authorization.Users;
    using Abp.Events.Bus;
    using Abp.Events.Bus.Entities;
    using Abp.MultiTenancy;
    using Abp.Runtime.Session;
    using Abp.TestBase;
    using AcmStatisticsAbp.Authorization.Users;
    using AcmStatisticsAbp.EntityFrameworkCore;
    using AcmStatisticsAbp.EntityFrameworkCore.Seed.Host;
    using AcmStatisticsAbp.EntityFrameworkCore.Seed.Tenants;
    using AcmStatisticsAbp.MultiTenancy;
    using Microsoft.EntityFrameworkCore;

    public abstract class AcmStatisticsAbpTestBase : AbpIntegratedTestBase<AcmStatisticsAbpTestModule>
    {
        protected AcmStatisticsAbpTestBase()
        {
            void NormalizeDbContext(AcmStatisticsAbpDbContext context)
            {
                context.EntityChangeEventHelper = NullEntityChangeEventHelper.Instance;
                context.EventBus = NullEventBus.Instance;
                context.SuppressAutoSetTenantId = true;
            }

            // Seed initial data for host
            this.AbpSession.TenantId = null;
            this.UsingDbContext(context =>
            {
                NormalizeDbContext(context);
                new InitialHostDbBuilder(context).Create();
                new DefaultTenantBuilder(context).Create();
            });

            // Seed initial data for default tenant
            this.AbpSession.TenantId = 1;
            this.UsingDbContext(context =>
            {
                NormalizeDbContext(context);
                new TenantRoleAndUserBuilder(context, 1).Create();
            });

            this.LoginAsDefaultTenantAdmin();
        }

        #region UsingDbContext

        protected IDisposable UsingTenantId(int? tenantId)
        {
            var previousTenantId = this.AbpSession.TenantId;
            this.AbpSession.TenantId = tenantId;
            return new DisposeAction(() => this.AbpSession.TenantId = previousTenantId);
        }

        protected void UsingDbContext(Action<AcmStatisticsAbpDbContext> action)
        {
            this.UsingDbContext(this.AbpSession.TenantId, action);
        }

        protected Task UsingDbContextAsync(Func<AcmStatisticsAbpDbContext, Task> action)
        {
            return this.UsingDbContextAsync(this.AbpSession.TenantId, action);
        }

        protected T UsingDbContext<T>(Func<AcmStatisticsAbpDbContext, T> func)
        {
            return this.UsingDbContext(this.AbpSession.TenantId, func);
        }

        protected Task<T> UsingDbContextAsync<T>(Func<AcmStatisticsAbpDbContext, Task<T>> func)
        {
            return this.UsingDbContextAsync(this.AbpSession.TenantId, func);
        }

        protected void UsingDbContext(int? tenantId, Action<AcmStatisticsAbpDbContext> action)
        {
            using (this.UsingTenantId(tenantId))
            {
                using (var context = this.LocalIocManager.Resolve<AcmStatisticsAbpDbContext>())
                {
                    action(context);
                    context.SaveChanges();
                }
            }
        }

        protected async Task UsingDbContextAsync(int? tenantId, Func<AcmStatisticsAbpDbContext, Task> action)
        {
            using (this.UsingTenantId(tenantId))
            {
                using (var context = this.LocalIocManager.Resolve<AcmStatisticsAbpDbContext>())
                {
                    await action(context);
                    await context.SaveChangesAsync();
                }
            }
        }

        protected T UsingDbContext<T>(int? tenantId, Func<AcmStatisticsAbpDbContext, T> func)
        {
            T result;

            using (this.UsingTenantId(tenantId))
            {
                using (var context = this.LocalIocManager.Resolve<AcmStatisticsAbpDbContext>())
                {
                    result = func(context);
                    context.SaveChanges();
                }
            }

            return result;
        }

        protected async Task<T> UsingDbContextAsync<T>(int? tenantId, Func<AcmStatisticsAbpDbContext, Task<T>> func)
        {
            T result;

            using (this.UsingTenantId(tenantId))
            {
                using (var context = this.LocalIocManager.Resolve<AcmStatisticsAbpDbContext>())
                {
                    result = await func(context);
                    await context.SaveChangesAsync();
                }
            }

            return result;
        }

        #endregion

        #region Login

        protected void LoginAsHostAdmin()
        {
            this.LoginAsHost(AbpUserBase.AdminUserName);
        }

        protected void LoginAsDefaultTenantAdmin()
        {
            this.LoginAsTenant(AbpTenantBase.DefaultTenantName, AbpUserBase.AdminUserName);
        }

        protected void LoginAsHost(string userName)
        {
            this.AbpSession.TenantId = null;

            var user =
                this.UsingDbContext(
                    context =>
                        context.Users.FirstOrDefault(u => u.TenantId == this.AbpSession.TenantId && u.UserName == userName));
            if (user == null)
            {
                throw new Exception("There is no user: " + userName + " for host.");
            }

            this.AbpSession.UserId = user.Id;
        }

        protected void LoginAsTenant(string tenancyName, string userName)
        {
            var tenant = this.UsingDbContext(context => context.Tenants.FirstOrDefault(t => t.TenancyName == tenancyName));
            if (tenant == null)
            {
                throw new Exception("There is no tenant: " + tenancyName);
            }

            this.AbpSession.TenantId = tenant.Id;

            var user =
                this.UsingDbContext(
                    context =>
                        context.Users.FirstOrDefault(u => u.TenantId == this.AbpSession.TenantId && u.UserName == userName));
            if (user == null)
            {
                throw new Exception("There is no user: " + userName + " for tenant: " + tenancyName);
            }

            this.AbpSession.UserId = user.Id;
        }

        #endregion

        /// <summary>
        /// Gets current user if <see cref="IAbpSession.UserId"/> is not null.
        /// Throws exception if it's null.
        /// </summary>
        protected async Task<User> GetCurrentUserAsync()
        {
            var userId = this.AbpSession.GetUserId();
            return await this.UsingDbContext(context => context.Users.SingleAsync(u => u.Id == userId));
        }

        /// <summary>
        /// Gets current tenant if <see cref="IAbpSession.TenantId"/> is not null.
        /// Throws exception if there is no current tenant.
        /// </summary>
        protected async Task<Tenant> GetCurrentTenantAsync()
        {
            var tenantId = this.AbpSession.GetTenantId();
            return await this.UsingDbContext(context => context.Tenants.SingleAsync(t => t.Id == tenantId));
        }
    }
}
