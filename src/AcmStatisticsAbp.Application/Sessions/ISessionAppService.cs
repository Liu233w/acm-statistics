// <copyright file="ISessionAppService.cs" company="西北工业大学ACM开发组">
// Copyright (c) 西北工业大学ACM开发组. All rights reserved.
// </copyright>

namespace AcmStatisticsAbp.Sessions
{
    using System.Threading.Tasks;
    using Abp.Application.Services;
    using AcmStatisticsAbp.Sessions.Dto;

    public interface ISessionAppService : IApplicationService
    {
        Task<GetCurrentLoginInformationsOutput> GetCurrentLoginInformations();
    }
}
