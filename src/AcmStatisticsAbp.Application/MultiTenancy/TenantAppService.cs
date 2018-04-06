// <copyright file="TenantAppService.cs" company="西北工业大学ACM开发组">
// Copyright (c) 西北工业大学ACM开发组. All rights reserved.
// </copyright>

namespace AcmStatisticsAbp.MultiTenancy
{
    using System.Linq;
    using System.Threading.Tasks;
    using Abp.Application.Services;
    using Abp.Application.Services.Dto;
    using Abp.Authorization;
    using Abp.Domain.Repositories;
    using Abp.Extensions;
    using Abp.IdentityFramework;
    using Abp.MultiTenancy;
    using Abp.Runtime.Security;
    using AcmStatisticsAbp.Authorization;
    using AcmStatisticsAbp.Authorization.Roles;
    using AcmStatisticsAbp.Authorization.Users;
    using AcmStatisticsAbp.Editions;
    using AcmStatisticsAbp.MultiTenancy.Dto;
    using Microsoft.AspNetCore.Identity;

    [AbpAuthorize(PermissionNames.Pages_Tenants)]
    public class TenantAppService : AsyncCrudAppService<Tenant, TenantDto, int, PagedResultRequestDto, CreateTenantDto, TenantDto>, ITenantAppService
    {
        private readonly TenantManager tenantManager;
        private readonly EditionManager editionManager;
        private readonly UserManager userManager;
        private readonly RoleManager roleManager;
        private readonly IAbpZeroDbMigrator abpZeroDbMigrator;
        private readonly IPasswordHasher<User> passwordHasher;

        public TenantAppService(
            IRepository<Tenant, int> repository,
            TenantManager tenantManager,
            EditionManager editionManager,
            UserManager userManager,
            RoleManager roleManager,
            IAbpZeroDbMigrator abpZeroDbMigrator,
            IPasswordHasher<User> passwordHasher)
            : base(repository)
        {
            this.tenantManager = tenantManager;
            this.editionManager = editionManager;
            this.userManager = userManager;
            this.roleManager = roleManager;
            this.abpZeroDbMigrator = abpZeroDbMigrator;
            this.passwordHasher = passwordHasher;
        }

        public override async Task<TenantDto> Create(CreateTenantDto input)
        {
            this.CheckCreatePermission();

            // Create tenant
            var tenant = this.ObjectMapper.Map<Tenant>(input);
            tenant.ConnectionString = input.ConnectionString.IsNullOrEmpty()
                ? null
                : SimpleStringCipher.Instance.Encrypt(input.ConnectionString);

            var defaultEdition = await this.editionManager.FindByNameAsync(EditionManager.DefaultEditionName);
            if (defaultEdition != null)
            {
                tenant.EditionId = defaultEdition.Id;
            }

            await this.tenantManager.CreateAsync(tenant);
            await this.CurrentUnitOfWork.SaveChangesAsync(); // To get new tenant's id.

            // Create tenant database
            this.abpZeroDbMigrator.CreateOrMigrateForTenant(tenant);

            // We are working entities of new tenant, so changing tenant filter
            using (this.CurrentUnitOfWork.SetTenantId(tenant.Id))
            {
                // Create static roles for new tenant
                this.CheckErrors(await this.roleManager.CreateStaticRoles(tenant.Id));

                await this.CurrentUnitOfWork.SaveChangesAsync(); // To get static role ids

                // Grant all permissions to admin role
                var adminRole = this.roleManager.Roles.Single(r => r.Name == StaticRoleNames.Tenants.Admin);
                await this.roleManager.GrantAllPermissionsAsync(adminRole);

                // Create admin user for the tenant
                var adminUser = User.CreateTenantAdminUser(tenant.Id, input.AdminEmailAddress);
                adminUser.Password = this.passwordHasher.HashPassword(adminUser, User.DefaultPassword);
                this.CheckErrors(await this.userManager.CreateAsync(adminUser));
                await this.CurrentUnitOfWork.SaveChangesAsync(); // To get admin user's id

                // Assign admin user to role!
                this.CheckErrors(await this.userManager.AddToRoleAsync(adminUser, adminRole.Name));
                await this.CurrentUnitOfWork.SaveChangesAsync();
            }

            return this.MapToEntityDto(tenant);
        }

        protected override void MapToEntity(TenantDto updateInput, Tenant entity)
        {
            // Manually mapped since TenantDto contains non-editable properties too.
            entity.Name = updateInput.Name;
            entity.TenancyName = updateInput.TenancyName;
            entity.IsActive = updateInput.IsActive;
        }

        public override async Task Delete(EntityDto<int> input)
        {
            this.CheckDeletePermission();

            var tenant = await this.tenantManager.GetByIdAsync(input.Id);
            await this.tenantManager.DeleteAsync(tenant);
        }

        private void CheckErrors(IdentityResult identityResult)
        {
            identityResult.CheckErrors(this.LocalizationManager);
        }
    }
}
