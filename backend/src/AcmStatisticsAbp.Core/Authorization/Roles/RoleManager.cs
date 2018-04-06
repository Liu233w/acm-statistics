// <copyright file="RoleManager.cs" company="西北工业大学ACM开发组">
// Copyright (c) 西北工业大学ACM开发组. All rights reserved.
// </copyright>

namespace AcmStatisticsAbp.Authorization.Roles
{
    using System.Collections.Generic;
    using Abp.Authorization;
    using Abp.Authorization.Roles;
    using Abp.Domain.Uow;
    using Abp.Runtime.Caching;
    using Abp.Zero.Configuration;
    using AcmStatisticsAbp.Authorization.Users;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.Extensions.Logging;

    public class RoleManager : AbpRoleManager<Role, User>
    {
        public RoleManager(
            RoleStore store,
            IEnumerable<IRoleValidator<Role>> roleValidators,
            ILookupNormalizer keyNormalizer,
            IdentityErrorDescriber errors,
            ILogger<AbpRoleManager<Role, User>> logger,
            IPermissionManager permissionManager,
            ICacheManager cacheManager,
            IUnitOfWorkManager unitOfWorkManager,
            IRoleManagementConfig roleManagementConfig)
            : base(
                  store,
                  roleValidators,
                  keyNormalizer,
                  errors,
                  logger,
                  permissionManager,
                  cacheManager,
                  unitOfWorkManager,
                  roleManagementConfig)
        {
        }
    }
}
