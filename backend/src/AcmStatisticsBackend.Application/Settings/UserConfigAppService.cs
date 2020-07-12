using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Authorization;
using Abp.Configuration;
using Abp.Dependency;
using Abp.Domain.Repositories;
using Abp.Runtime.Session;
using Abp.Timing;
using Abp.UI;
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
        private readonly IClockProvider _clockProvider;
        private readonly IRepository<UserSettingAttribute, long> _userSettingAttributeRepository;

        public UserConfigAppService(ISettingDefinitionManager settingDefinitionManager, IIocResolver iocResolver, IClockProvider clockProvider, IRepository<UserSettingAttribute, long> userSettingAttributeRepository)
        {
            _settingDefinitionManager = settingDefinitionManager;
            _iocResolver = iocResolver;
            _clockProvider = clockProvider;
            _userSettingAttributeRepository = userSettingAttributeRepository;
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

        /// <inheritdoc />
        [AbpAuthorize(PermissionNames.Settings_All)]
        public async Task SetUserTimeZone(UserTimeZoneDto dto)
        {
            var settings = await GetOrCreateUserSettingAttribute();

            if (settings.LastTimeZoneChangedTime.HasValue
                && settings.LastTimeZoneChangedTime.Value.AddDays(1) > _clockProvider.Now)
            {
                throw new UserFriendlyException("Please wait 24 hours to set time zone again!");
            }

            await SettingManager.ChangeSettingForUserAsync(
                AbpSession.ToUserIdentifier(),
                TimingSettingNames.TimeZone,
                dto.TimeZone);
            settings.LastTimeZoneChangedTime = _clockProvider.Now;
        }

        private async Task<UserSettingAttribute> GetOrCreateUserSettingAttribute()
        {
            return await _userSettingAttributeRepository.FirstOrDefaultAsync(
                       item => item.UserId == AbpSession.UserId.Value)
                   ??
                   await _userSettingAttributeRepository.InsertAsync(
                       new UserSettingAttribute
                       {
                           UserId = AbpSession.GetUserId(),
                       });
        }
    }
}
