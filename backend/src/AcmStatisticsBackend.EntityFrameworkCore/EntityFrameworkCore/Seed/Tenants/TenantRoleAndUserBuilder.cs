using System.Linq;
using Abp.Authorization;
using Abp.Authorization.Roles;
using Abp.Authorization.Users;
using Abp.MultiTenancy;
using AcmStatisticsBackend.Authorization;
using AcmStatisticsBackend.Authorization.Roles;
using AcmStatisticsBackend.Authorization.Users;
using AcmStatisticsBackend.Configuration;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace AcmStatisticsBackend.EntityFrameworkCore.Seed.Tenants
{
    public class TenantRoleAndUserBuilder
    {
        private readonly AcmStatisticsBackendDbContext _context;
        private readonly int _tenantId;

        public TenantRoleAndUserBuilder(AcmStatisticsBackendDbContext context, int tenantId)
        {
            _context = context;
            _tenantId = tenantId;
        }

        public void Create()
        {
            CreateAdminRoleAndUser();
            CreateUserRole();
        }

        private void CreateAdminRoleAndUser()
        {
            // Admin role

            var adminRole = _context.Roles.IgnoreQueryFilters()
                .FirstOrDefault(r => r.TenantId == _tenantId && r.Name == StaticRoleNames.Tenants.Admin);
            if (adminRole == null)
            {
                adminRole = _context.Roles
                    .Add(new Role(_tenantId, StaticRoleNames.Tenants.Admin, StaticRoleNames.Tenants.Admin)
                    {
                        IsStatic = true,
                    }).Entity;
                _context.SaveChanges();
            }

            // Grant all permissions to admin role

            var grantedPermissions = _context.Permissions.IgnoreQueryFilters()
                .OfType<RolePermissionSetting>()
                .Where(p => p.TenantId == _tenantId && p.RoleId == adminRole.Id)
                .Select(p => p.Name)
                .ToList();

            var permissions = PermissionFinder
                .GetAllPermissions(new AcmStatisticsBackendAuthorizationProvider())
                .Where(p => p.MultiTenancySides.HasFlag(MultiTenancySides.Tenant) &&
                            !grantedPermissions.Contains(p.Name))
                .ToList();

            if (permissions.Any())
            {
                _context.Permissions.AddRange(
                    permissions.Select(permission => new RolePermissionSetting
                    {
                        TenantId = _tenantId,
                        Name = permission.Name,
                        IsGranted = true,
                        RoleId = adminRole.Id,
                    }));
                _context.SaveChanges();
            }

            // Admin user

            var adminUser = _context.Users.IgnoreQueryFilters()
                .FirstOrDefault(u => u.TenantId == _tenantId && u.UserName == AbpUserBase.AdminUserName);
            if (adminUser == null)
            {
                adminUser = User.CreateTenantAdminUser(_tenantId, "admin@defaulttenant.com");
                var adminPassword = AppEnvironmentVariables.DefaultAdminPassword;
                adminUser.Password =
                    new PasswordHasher<User>(new OptionsWrapper<PasswordHasherOptions>(new PasswordHasherOptions()))
                        .HashPassword(adminUser, adminPassword);
                adminUser.IsEmailConfirmed = true;
                adminUser.IsActive = true;

                _context.Users.Add(adminUser);
                _context.SaveChanges();

                // Assign Admin role to admin user
                _context.UserRoles.Add(new UserRole(_tenantId, adminUser.Id, adminRole.Id));
                _context.SaveChanges();
            }
        }

        private void CreateUserRole()
        {
            // set User role as default role
            var userRole = _context.Roles.IgnoreQueryFilters()
                .FirstOrDefault(r => r.TenantId == _tenantId && r.Name == StaticRoleNames.Tenants.User);
            if (userRole == null)
            {
                userRole = _context.Roles.Add(
                    new Role(_tenantId, StaticRoleNames.Tenants.User, StaticRoleNames.Tenants.User)
                    {
                        IsStatic = true,
                        IsDefault = true,
                    }).Entity;
                _context.SaveChanges();
            }

            GrantPermissionForRule(userRole, new[]
            {
                PermissionNames.Statistics_DefaultQuery,
                PermissionNames.AcHistory_Histories,
                PermissionNames.Settings_Update,
            });
        }

        private void GrantPermissionForRule(Role role, params string[] permissionNames)
        {
            var granted = _context.RolePermissions
                .Where(item => item.TenantId == _tenantId && item.RoleId == role.Id)
                .Select(item => item.Name);

            var shouldGrant = permissionNames.Except(granted);
            foreach (var name in shouldGrant)
            {
                _context.Permissions.Add(new RolePermissionSetting
                {
                    RoleId = role.Id,
                    TenantId = _tenantId,
                    IsGranted = true,
                    Name = name,
                });
            }

            _context.SaveChanges();
        }
    }
}
