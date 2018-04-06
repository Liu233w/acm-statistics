// <copyright file="IUserAppService.cs" company="西北工业大学ACM开发组">
// Copyright (c) 西北工业大学ACM开发组. All rights reserved.
// </copyright>

namespace AcmStatisticsAbp.Users
{
    using System.Threading.Tasks;
    using Abp.Application.Services;
    using Abp.Application.Services.Dto;
    using AcmStatisticsAbp.Roles.Dto;
    using AcmStatisticsAbp.Users.Dto;

    public interface IUserAppService : IAsyncCrudAppService<UserDto, long, PagedResultRequestDto, CreateUserDto, UserDto>
    {
        Task<ListResultDto<RoleDto>> GetRoles();

        Task ChangeLanguage(ChangeUserLanguageDto input);
    }
}
