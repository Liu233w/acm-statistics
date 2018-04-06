// <copyright file="UserStore.cs" company="西北工业大学ACM开发组">
// Copyright (c) 西北工业大学ACM开发组. All rights reserved.
// </copyright>

namespace AcmStatisticsAbp.Authorization.Users
{
    using Abp.Authorization.Users;
    using Abp.Domain.Repositories;
    using Abp.Domain.Uow;
    using Abp.Linq;
    using AcmStatisticsAbp.Authorization.Roles;

    public class UserStore : AbpUserStore<Role, User>
    {
        public UserStore(
            IUnitOfWorkManager unitOfWorkManager,
            IRepository<User, long> userRepository,
            IRepository<Role> roleRepository,
            IAsyncQueryableExecuter asyncQueryableExecuter,
            IRepository<UserRole, long> userRoleRepository,
            IRepository<UserLogin, long> userLoginRepository,
            IRepository<UserClaim, long> userClaimRepository,
            IRepository<UserPermissionSetting, long> userPermissionSettingRepository)
            : base(
                  unitOfWorkManager,
                  userRepository,
                  roleRepository,
                  asyncQueryableExecuter,
                  userRoleRepository,
                  userLoginRepository,
                  userClaimRepository,
                  userPermissionSettingRepository)
        {
        }
    }
}
