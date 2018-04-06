// <copyright file="ConfigurationAppService.cs" company="西北工业大学ACM开发组">
// Copyright (c) 西北工业大学ACM开发组. All rights reserved.
// </copyright>

namespace AcmStatisticsAbp.Configuration
{
    using System.Threading.Tasks;
    using Abp.Authorization;
    using Abp.Runtime.Session;
    using AcmStatisticsAbp.Configuration.Dto;

    [AbpAuthorize]
    public class ConfigurationAppService : AcmStatisticsAbpAppServiceBase, IConfigurationAppService
    {
        public async Task ChangeUiTheme(ChangeUiThemeInput input)
        {
            await this.SettingManager.ChangeSettingForUserAsync(this.AbpSession.ToUserIdentifier(), AppSettingNames.UiTheme, input.Theme);
        }
    }
}
