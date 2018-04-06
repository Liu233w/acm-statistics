// <copyright file="RoleMapProfile.cs" company="西北工业大学ACM开发组">
// Copyright (c) 西北工业大学ACM开发组. All rights reserved.
// </copyright>

namespace AcmStatisticsAbp.Roles.Dto
{
    using Abp.Authorization;
    using Abp.Authorization.Roles;
    using AcmStatisticsAbp.Authorization.Roles;
    using AutoMapper;

    public class RoleMapProfile : Profile
    {
        public RoleMapProfile()
        {
            // Role and permission
            this.CreateMap<Permission, string>().ConvertUsing(r => r.Name);
            this.CreateMap<RolePermissionSetting, string>().ConvertUsing(r => r.Name);

            this.CreateMap<CreateRoleDto, Role>().ForMember(x => x.Permissions, opt => opt.Ignore());
            this.CreateMap<RoleDto, Role>().ForMember(x => x.Permissions, opt => opt.Ignore());
        }
    }
}
