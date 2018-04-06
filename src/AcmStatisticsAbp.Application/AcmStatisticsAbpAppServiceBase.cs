// <copyright file="AcmStatisticsAbpAppServiceBase.cs" company="西北工业大学ACM开发组">
// Copyright (c) 西北工业大学ACM开发组. All rights reserved.
// </copyright>

namespace AcmStatisticsAbp
{
    using System;
    using System.Threading.Tasks;
    using Abp.Application.Services;
    using Abp.IdentityFramework;
    using Abp.Runtime.Session;
    using AcmStatisticsAbp.Authorization.Users;
    using AcmStatisticsAbp.MultiTenancy;
    using Microsoft.AspNetCore.Identity;

    /// <summary>
    /// Derive your application services from this class.
    /// </summary>
    public abstract class AcmStatisticsAbpAppServiceBase : ApplicationService
    {
        public TenantManager TenantManager { get; set; }

        public UserManager UserManager { get; set; }

        protected AcmStatisticsAbpAppServiceBase()
        {
            this.LocalizationSourceName = AcmStatisticsAbpConsts.LocalizationSourceName;
        }

        protected virtual Task<User> GetCurrentUserAsync()
        {
            var user = this.UserManager.FindByIdAsync(this.AbpSession.GetUserId().ToString());
            if (user == null)
            {
                throw new Exception("There is no current user!");
            }

            return user;
        }

        protected virtual Task<Tenant> GetCurrentTenantAsync()
        {
            return this.TenantManager.GetByIdAsync(this.AbpSession.GetTenantId());
        }

        protected virtual void CheckErrors(IdentityResult identityResult)
        {
            identityResult.CheckErrors(this.LocalizationManager);
        }
    }
}
