// <copyright file="RoleDto.cs" company="西北工业大学ACM开发组">
// Copyright (c) 西北工业大学ACM开发组. All rights reserved.
// </copyright>

namespace AcmStatisticsAbp.Roles.Dto
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using Abp.Application.Services.Dto;
    using Abp.Authorization.Roles;
    using Abp.AutoMapper;
    using AcmStatisticsAbp.Authorization.Roles;

    [AutoMap(typeof(Role))]
    public class RoleDto : EntityDto<int>
    {
        [Required]
        [StringLength(AbpRoleBase.MaxNameLength)]
        public string Name { get; set; }

        [Required]
        [StringLength(AbpRoleBase.MaxDisplayNameLength)]
        public string DisplayName { get; set; }

        public string NormalizedName { get; set; }

        [StringLength(Role.MaxDescriptionLength)]
        public string Description { get; set; }

        public bool IsStatic { get; set; }

        public List<string> Permissions { get; set; }
    }
}
