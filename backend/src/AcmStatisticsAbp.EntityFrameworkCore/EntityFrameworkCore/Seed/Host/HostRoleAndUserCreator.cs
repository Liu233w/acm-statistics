// <copyright file="HostRoleAndUserCreator.cs" company="西北工业大学ACM开发组">
// Copyright (c) 西北工业大学ACM开发组. All rights reserved.
// </copyright>

namespace AcmStatisticsAbp.EntityFrameworkCore.Seed.Host
{
    using System.Linq;
    using Abp.Authorization;
    using Abp.Authorization.Roles;
    using Abp.Authorization.Users;
    using Abp.MultiTenancy;
    using AcmStatisticsAbp.Authorization;
    using AcmStatisticsAbp.Authorization.Roles;
    using AcmStatisticsAbp.Authorization.Users;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Options;

    public class HostRoleAndUserCreator
    {
        private readonly AcmStatisticsAbpDbContext context;

        public HostRoleAndUserCreator(AcmStatisticsAbpDbContext context)
        {
            this.context = context;
        }

        public void Create()
        {
            this.CreateHostRoleAndUsers();
        }

        private void CreateHostRoleAndUsers()
        {
            // Admin role for host
            var adminRoleForHost = this.context.Roles.IgnoreQueryFilters().FirstOrDefault(r => r.TenantId == null && r.Name == StaticRoleNames.Host.Admin);
            if (adminRoleForHost == null)
            {
                adminRoleForHost = this.context.Roles.Add(new Role(null, StaticRoleNames.Host.Admin, StaticRoleNames.Host.Admin) { IsStatic = true, IsDefault = true }).Entity;
                this.context.SaveChanges();
            }

            // Grant all permissions to admin role for host
            var grantedPermissions = this.context.Permissions.IgnoreQueryFilters()
                .OfType<RolePermissionSetting>()
                .Where(p => p.TenantId == null && p.RoleId == adminRoleForHost.Id)
                .Select(p => p.Name)
                .ToList();

            var permissions = PermissionFinder
                .GetAllPermissions(new AcmStatisticsAbpAuthorizationProvider())
                .Where(p => p.MultiTenancySides.HasFlag(MultiTenancySides.Host) &&
                            !grantedPermissions.Contains(p.Name))
                .ToList();

            if (permissions.Any())
            {
                this.context.Permissions.AddRange(
                    permissions.Select(permission => new RolePermissionSetting
                    {
                        TenantId = null,
                        Name = permission.Name,
                        IsGranted = true,
                        RoleId = adminRoleForHost.Id,
                    }));
                this.context.SaveChanges();
            }

            // Admin user for host
            var adminUserForHost = this.context.Users.IgnoreQueryFilters().FirstOrDefault(u => u.TenantId == null && u.UserName == AbpUserBase.AdminUserName);
            if (adminUserForHost == null)
            {
                var user = new User
                {
                    TenantId = null,
                    UserName = AbpUserBase.AdminUserName,
                    Name = "admin",
                    Surname = "admin",
                    EmailAddress = "admin@aspnetboilerplate.com",
                    IsEmailConfirmed = true,
                    IsActive = true,
                };

                user.Password = new PasswordHasher<User>(new OptionsWrapper<PasswordHasherOptions>(new PasswordHasherOptions())).HashPassword(user, "123qwe");
                user.SetNormalizedNames();

                adminUserForHost = this.context.Users.Add(user).Entity;
                this.context.SaveChanges();

                // Assign Admin role to admin user
                this.context.UserRoles.Add(new UserRole(null, adminUserForHost.Id, adminRoleForHost.Id));
                this.context.SaveChanges();

                // User account of admin user
                this.context.UserAccounts.Add(new UserAccount
                {
                    TenantId = null,
                    UserId = adminUserForHost.Id,
                    UserName = AbpUserBase.AdminUserName,
                    EmailAddress = adminUserForHost.EmailAddress,
                });
                this.context.SaveChanges();
            }
        }
    }
}
