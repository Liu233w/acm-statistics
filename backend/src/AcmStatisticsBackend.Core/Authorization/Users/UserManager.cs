using System;
using System.Collections.Generic;
using Abp.Authorization;
using Abp.Authorization.Roles;
using Abp.Authorization.Users;
using Abp.Configuration;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Organizations;
using Abp.Runtime.Caching;
using AcmStatisticsBackend.Authorization.Roles;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AcmStatisticsBackend.Authorization.Users
{
    public class UserManager : AbpUserManager<Role, User>
    {
        public UserManager(AbpRoleManager<Role, User> roleManager, AbpUserStore<Role, User> userStore, IOptions<IdentityOptions> optionsAccessor, IPasswordHasher<User> passwordHasher, IEnumerable<IUserValidator<User>> userValidators, IEnumerable<IPasswordValidator<User>> passwordValidators, ILookupNormalizer keyNormalizer, IdentityErrorDescriber errors, IServiceProvider services, ILogger<UserManager<User>> logger, IPermissionManager permissionManager, IUnitOfWorkManager unitOfWorkManager, ICacheManager cacheManager, IRepository<OrganizationUnit, long> organizationUnitRepository, IRepository<UserOrganizationUnit, long> userOrganizationUnitRepository, IOrganizationUnitSettings organizationUnitSettings, ISettingManager settingManager, IRepository<UserLogin, long> userLoginRepository) : base(roleManager, userStore, optionsAccessor, passwordHasher, userValidators, passwordValidators, keyNormalizer, errors, services, logger, permissionManager, unitOfWorkManager, cacheManager, organizationUnitRepository, userOrganizationUnitRepository, organizationUnitSettings, settingManager, userLoginRepository)
        {
        }
    }
}
