// <copyright file="RoleAppService.cs" company="西北工业大学ACM开发组">
// Copyright (c) 西北工业大学ACM开发组. All rights reserved.
// </copyright>

namespace AcmStatisticsAbp.Roles
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Abp.Application.Services;
    using Abp.Application.Services.Dto;
    using Abp.Authorization;
    using Abp.Domain.Repositories;
    using Abp.IdentityFramework;
    using Abp.UI;
    using AcmStatisticsAbp.Authorization;
    using AcmStatisticsAbp.Authorization.Roles;
    using AcmStatisticsAbp.Authorization.Users;
    using AcmStatisticsAbp.Roles.Dto;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;

    [AbpAuthorize(PermissionNames.Pages_Roles)]
    public class RoleAppService : AsyncCrudAppService<Role, RoleDto, int, PagedResultRequestDto, CreateRoleDto, RoleDto>, IRoleAppService
    {
        private readonly RoleManager roleManager;
        private readonly UserManager userManager;

        public RoleAppService(IRepository<Role> repository, RoleManager roleManager, UserManager userManager)
            : base(repository)
        {
            this.roleManager = roleManager;
            this.userManager = userManager;
        }

        public override async Task<RoleDto> Create(CreateRoleDto input)
        {
            this.CheckCreatePermission();

            var role = this.ObjectMapper.Map<Role>(input);
            role.SetNormalizedName();

            this.CheckErrors(await this.roleManager.CreateAsync(role));

            var grantedPermissions = this.PermissionManager
                .GetAllPermissions()
                .Where(p => input.Permissions.Contains(p.Name))
                .ToList();

            await this.roleManager.SetGrantedPermissionsAsync(role, grantedPermissions);

            return this.MapToEntityDto(role);
        }

        public override async Task<RoleDto> Update(RoleDto input)
        {
            this.CheckUpdatePermission();

            var role = await this.roleManager.GetRoleByIdAsync(input.Id);

            this.ObjectMapper.Map(input, role);

            this.CheckErrors(await this.roleManager.UpdateAsync(role));

            var grantedPermissions = this.PermissionManager
                .GetAllPermissions()
                .Where(p => input.Permissions.Contains(p.Name))
                .ToList();

            await this.roleManager.SetGrantedPermissionsAsync(role, grantedPermissions);

            return this.MapToEntityDto(role);
        }

        public override async Task Delete(EntityDto<int> input)
        {
            this.CheckDeletePermission();

            var role = await this.roleManager.FindByIdAsync(input.Id.ToString());
            var users = await this.userManager.GetUsersInRoleAsync(role.NormalizedName);

            foreach (var user in users)
            {
                this.CheckErrors(await this.userManager.RemoveFromRoleAsync(user, role.NormalizedName));
            }

            this.CheckErrors(await this.roleManager.DeleteAsync(role));
        }

        public Task<ListResultDto<PermissionDto>> GetAllPermissions()
        {
            var permissions = this.PermissionManager.GetAllPermissions();

            return Task.FromResult(new ListResultDto<PermissionDto>(
                this.ObjectMapper.Map<List<PermissionDto>>(permissions)));
        }

        protected override IQueryable<Role> CreateFilteredQuery(PagedResultRequestDto input)
        {
            return this.Repository.GetAllIncluding(x => x.Permissions);
        }

        protected override async Task<Role> GetEntityByIdAsync(int id)
        {
            return await this.Repository.GetAllIncluding(x => x.Permissions).FirstOrDefaultAsync(x => x.Id == id);
        }

        protected override IQueryable<Role> ApplySorting(IQueryable<Role> query, PagedResultRequestDto input)
        {
            return query.OrderBy(r => r.DisplayName);
        }

        protected virtual void CheckErrors(IdentityResult identityResult)
        {
            identityResult.CheckErrors(this.LocalizationManager);
        }
    }
}
