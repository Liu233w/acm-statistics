// <copyright file="IConfigurationAppService.cs" company="西北工业大学ACM开发组">
// Copyright (c) 西北工业大学ACM开发组. All rights reserved.
// </copyright>

namespace AcmStatisticsAbp.Configuration
{
    using System.Threading.Tasks;
    using AcmStatisticsAbp.Configuration.Dto;

    public interface IConfigurationAppService
    {
        Task ChangeUiTheme(ChangeUiThemeInput input);
    }
}
