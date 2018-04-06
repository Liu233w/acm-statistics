// <copyright file="ITenantAppService.cs" company="西北工业大学ACM开发组">
// Copyright (c) 西北工业大学ACM开发组. All rights reserved.
// </copyright>

namespace AcmStatisticsAbp.MultiTenancy
{
    using Abp.Application.Services;
    using Abp.Application.Services.Dto;
    using AcmStatisticsAbp.MultiTenancy.Dto;

    public interface ITenantAppService : IAsyncCrudAppService<TenantDto, int, PagedResultRequestDto, CreateTenantDto, TenantDto>
    {
    }
}
