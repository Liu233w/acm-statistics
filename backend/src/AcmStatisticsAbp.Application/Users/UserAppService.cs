// <copyright file="UserAppService.cs" company="西北工业大学ACM开发组">
// Copyright (c) 西北工业大学ACM开发组. All rights reserved.
// </copyright>

namespace AcmStatisticsAbp.Users
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Abp.Application.Services;
    using Abp.Application.Services.Dto;
    using Abp.Authorization;
    using Abp.Domain.Repositories;
    using Abp.IdentityFramework;
    using Abp.Localization;
    using Abp.Runtime.Session;
    using AcmStatisticsAbp.Authorization;
    using AcmStatisticsAbp.Authorization.Roles;
    using AcmStatisticsAbp.Authorization.Users;
    using AcmStatisticsAbp.Roles.Dto;
    using AcmStatisticsAbp.Users.Dto;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;

    [AbpAuthorize(PermissionNames.Pages_Users)]
    public class UserAppService : AsyncCrudAppService<User, UserDto, long, PagedResultRequestDto, CreateUserDto, UserDto>, IUserAppService
    {
        private readonly UserManager userManager;
        private readonly RoleManager roleManager;
        private readonly IRepository<Role> roleRepository;
        private readonly IPasswordHasher<User> passwordHasher;

        public UserAppService(
            IRepository<User, long> repository,
            UserManager userManager,
            RoleManager roleManager,
            IRepository<Role> roleRepository,
            IPasswordHasher<User> passwordHasher)
            : base(repository)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
            this.roleRepository = roleRepository;
            this.passwordHasher = passwordHasher;
        }

        public override async Task<UserDto> Create(CreateUserDto input)
        {
            this.CheckCreatePermission();

            var user = this.ObjectMapper.Map<User>(input);

            user.TenantId = this.AbpSession.TenantId;
            user.Password = this.passwordHasher.HashPassword(user, input.Password);
            user.IsEmailConfirmed = true;

            this.CheckErrors(await this.userManager.CreateAsync(user));

            if (input.RoleNames != null)
            {
                this.CheckErrors(await this.userManager.SetRoles(user, input.RoleNames));
            }

            this.CurrentUnitOfWork.SaveChanges();

            return this.MapToEntityDto(user);
        }

        public override async Task<UserDto> Update(UserDto input)
        {
            this.CheckUpdatePermission();

            var user = await this.userManager.GetUserByIdAsync(input.Id);

            this.MapToEntity(input, user);

            this.CheckErrors(await this.userManager.UpdateAsync(user));

            if (input.RoleNames != null)
            {
                this.CheckErrors(await this.userManager.SetRoles(user, input.RoleNames));
            }

            return await this.Get(input);
        }

        public override async Task Delete(EntityDto<long> input)
        {
            var user = await this.userManager.GetUserByIdAsync(input.Id);
            await this.userManager.DeleteAsync(user);
        }

        public async Task<ListResultDto<RoleDto>> GetRoles()
        {
            var roles = await this.roleRepository.GetAllListAsync();
            return new ListResultDto<RoleDto>(this.ObjectMapper.Map<List<RoleDto>>(roles));
        }

        public async Task ChangeLanguage(ChangeUserLanguageDto input)
        {
            await this.SettingManager.ChangeSettingForUserAsync(
                this.AbpSession.ToUserIdentifier(),
                LocalizationSettingNames.DefaultLanguage,
                input.LanguageName);
        }

        protected override User MapToEntity(CreateUserDto createInput)
        {
            var user = this.ObjectMapper.Map<User>(createInput);
            user.SetNormalizedNames();
            return user;
        }

        protected override void MapToEntity(UserDto input, User user)
        {
            this.ObjectMapper.Map(input, user);
            user.SetNormalizedNames();
        }

        protected override UserDto MapToEntityDto(User user)
        {
            var roles = this.roleManager.Roles.Where(r => user.Roles.Any(ur => ur.RoleId == r.Id)).Select(r => r.NormalizedName);
            var userDto = base.MapToEntityDto(user);
            userDto.RoleNames = roles.ToArray();
            return userDto;
        }

        protected override IQueryable<User> CreateFilteredQuery(PagedResultRequestDto input)
        {
            return this.Repository.GetAllIncluding(x => x.Roles);
        }

        protected override async Task<User> GetEntityByIdAsync(long id)
        {
            return await this.Repository.GetAllIncluding(x => x.Roles).FirstOrDefaultAsync(x => x.Id == id);
        }

        protected override IQueryable<User> ApplySorting(IQueryable<User> query, PagedResultRequestDto input)
        {
            return query.OrderBy(r => r.UserName);
        }

        protected virtual void CheckErrors(IdentityResult identityResult)
        {
            identityResult.CheckErrors(this.LocalizationManager);
        }
    }
}
