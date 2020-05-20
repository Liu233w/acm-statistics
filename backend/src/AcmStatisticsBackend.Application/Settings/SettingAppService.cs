using System;
using System.Linq;
using System.Threading.Tasks;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Runtime.Session;
using Abp.Timing;
using Abp.UI;
using AcmStatisticsBackend.Settings.Dto;
using AcmStatisticsBackend.Timing;

namespace AcmStatisticsBackend.Settings
{
    [AbpAuthorize]
    public class SettingAppService : AcmStatisticsBackendAppServiceBase, ISettingAppService
    {
        private readonly IClockProvider _clockProvider;
        private readonly UserTimeZoneManager _userTimeZoneManager;

        private readonly IRepository<UserSettingAttribute, long> _userSettingAttributeRepository;

        public SettingAppService(
            IClockProvider clockProvider,
            UserTimeZoneManager userTimeZoneManager,
            IRepository<UserSettingAttribute, long> userSettingAttributeRepository)
        {
            _clockProvider = clockProvider;
            _userTimeZoneManager = userTimeZoneManager;
            _userSettingAttributeRepository = userSettingAttributeRepository;
        }

        /// <inheritdoc />
        public async Task<UserTimeZoneDto> GetUserTimeZone()
        {
            return new UserTimeZoneDto
            {
                TimeZone = await _userTimeZoneManager.GetTimeZoneNameOfUserAsync(
                    AbpSession.ToUserIdentifier()),
            };
        }

        /// <inheritdoc />
        public async Task SetUserTimeZone(UserTimeZoneDto dto)
        {
            var settings = await GetOrCreateUserSettingAttribute();

            if (settings.LastTimeZoneChangedTime.HasValue
                && settings.LastTimeZoneChangedTime.Value.AddDays(1) > _clockProvider.Now)
            {
                throw new UserFriendlyException("Please wait 24 hours to set time zone again!");
            }

            await _userTimeZoneManager.SetTimeZoneOfUserAsync(AbpSession.ToUserIdentifier(), dto.TimeZone);
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
