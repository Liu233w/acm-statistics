﻿using System;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.IdentityFramework;
using Abp.Runtime.Session;
using AcmStatisticsBackend.Authorization.Users;
using AcmStatisticsBackend.MultiTenancy;
using Microsoft.AspNetCore.Identity;

namespace AcmStatisticsBackend
{
    /// <summary>
    /// Derive your application services from this class.
    /// </summary>
    public abstract class AcmStatisticsBackendAppServiceBase : ApplicationService
    {
        public TenantManager TenantManager { get; set; }

        public UserManager UserManager { get; set; }

        protected AcmStatisticsBackendAppServiceBase()
        {
            LocalizationSourceName = AcmStatisticsBackendConsts.LocalizationSourceName;
        }

        protected virtual async Task<User> GetCurrentUserAsync()
        {
            var user = await UserManager.FindByIdAsync(AbpSession.GetUserId().ToString());
            if (user == null)
            {
                throw new Exception("There is no current user!");
            }

            return user;
        }

        protected virtual Task<Tenant> GetCurrentTenantAsync()
        {
            return TenantManager.GetByIdAsync(AbpSession.GetTenantId());
        }

        protected virtual void CheckErrors(IdentityResult identityResult)
        {
            identityResult.CheckErrors(LocalizationManager);
        }
    }
}
