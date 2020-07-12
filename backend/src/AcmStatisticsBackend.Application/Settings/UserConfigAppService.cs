using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Authorization;
using Abp.Configuration;
using Abp.Dependency;
using Abp.Runtime.Session;
using AcmStatisticsBackend.Authorization;
using AcmStatisticsBackend.Configuration;
using AcmStatisticsBackend.Settings.Dto;

namespace AcmStatisticsBackend.Settings
{
    /// <inheritdoc cref="IUserConfigAppService"/>
    [AbpAuthorize]
    public class UserConfigAppService : AcmStatisticsBackendAppServiceBase, IUserConfigAppService
    {
        private readonly ISettingDefinitionManager _settingDefinitionManager;
        private readonly IIocResolver _iocResolver;

        public UserConfigAppService(ISettingDefinitionManager settingDefinitionManager, IIocResolver iocResolver)
        {
            _settingDefinitionManager = settingDefinitionManager;
            _iocResolver = iocResolver;
        }

        /// <inheritdoc cref="IUserConfigAppService.GetUserSettings"/>
        public async Task<UserSettingsConfigDto> GetUserSettings()
        {
            var config = new UserSettingsConfigDto
            {
                Values = new Dictionary<string, string>(),
            };

            var settings = await SettingManager.GetAllSettingValuesAsync(SettingScopes.All);

            using var scope = _iocResolver.CreateScope();
            foreach (var settingValue in settings)
            {
                if (!await _settingDefinitionManager.GetSettingDefinition(settingValue.Name)
                    .ClientVisibilityProvider
                    .CheckVisible(scope))
                {
                    continue;
                }

                config.Values.Add(settingValue.Name, settingValue.Value);
            }

            return config;
        }

        /// <inheritdoc/>
        [AbpAuthorize(PermissionNames.Settings_All)]
        public async Task UpdateAutoSaveHistory(UpdateAutoSaveHistoryInput input)
        {
            await SettingManager.ChangeSettingForUserAsync(
                AbpSession.ToUserIdentifier(),
                AppSettingNames.AutoSaveHistory,
                input.AutoSaveHistory ? "true" : "false");
        }
    }
}
