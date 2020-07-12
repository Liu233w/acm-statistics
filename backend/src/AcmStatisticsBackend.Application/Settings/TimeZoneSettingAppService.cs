using System.Threading.Tasks;
using Abp.Authorization;
using Abp.Configuration;
using Abp.Domain.Repositories;
using Abp.Runtime.Session;
using Abp.Timing;
using Abp.UI;
using AcmStatisticsBackend.Authorization;
using AcmStatisticsBackend.Settings.Dto;

namespace AcmStatisticsBackend.Settings
{
    [AbpAuthorize]
    public class TimeZoneSettingAppService : AcmStatisticsBackendAppServiceBase, ITimeZoneSettingAppService
    {
        private readonly IClockProvider _clockProvider;
        private readonly ISettingManager _settingManager;

        private readonly IRepository<UserSettingAttribute, long> _userSettingAttributeRepository;

        public TimeZoneSettingAppService(
            IClockProvider clockProvider,
            IRepository<UserSettingAttribute, long> userSettingAttributeRepository,
            ISettingManager settingManager)
        {
            _clockProvider = clockProvider;
            _userSettingAttributeRepository = userSettingAttributeRepository;
            _settingManager = settingManager;
        }

        /// <inheritdoc />
        public async Task<UserTimeZoneDto> GetUserTimeZone()
        {
            return new UserTimeZoneDto
            {
                TimeZone = await _settingManager.GetSettingValueForUserAsync(
                    TimingSettingNames.TimeZone, AbpSession.ToUserIdentifier()),
            };
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

            await _settingManager.ChangeSettingForUserAsync(
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
