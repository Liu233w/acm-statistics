using System.Threading;
using System.Threading.Tasks;
using Abp;
using Abp.Authorization.Users;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Linq;
using Abp.Organizations;
using AcmStatisticsBackend.Authorization.Roles;
using Microsoft.AspNetCore.Identity;

namespace AcmStatisticsBackend.Authorization.Users
{
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
            IRepository<UserPermissionSetting, long> userPermissionSettingRepository,
            IRepository<UserOrganizationUnit, long> userOrganizationUnitRepository,
            IRepository<OrganizationUnitRole, long> organizationUnitRoleRepository)
            : base(
                unitOfWorkManager,
                userRepository,
                roleRepository,
                asyncQueryableExecuter,
                userRoleRepository,
                userLoginRepository,
                userClaimRepository,
                userPermissionSettingRepository,
                userOrganizationUnitRepository,
                organizationUnitRoleRepository)
        {
        }

        public override async Task<IdentityResult> DeleteAsync(User user, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            Check.NotNull(user, nameof(user));

            await UserRepository.HardDeleteAsync(user);

            try
            {
                await SaveChangesAsync(cancellationToken);
            }
            catch (AbpDbConcurrencyException ex)
            {
                Logger.Warn(ex.ToString(), ex);
                return IdentityResult.Failed(ErrorDescriber.ConcurrencyFailure());
            }

            await SaveChangesAsync(cancellationToken);

            return IdentityResult.Success;
        }
    }
}
