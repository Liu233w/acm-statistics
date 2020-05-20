using System;
using System.Threading.Tasks;
using Abp;
using Abp.Configuration;
using Abp.Dependency;
using Abp.Timing;
using Abp.Timing.Timezone;
using TimeZoneConverter;

namespace AcmStatisticsBackend.Timing
{
    public class UserTimeZoneManager : ITransientDependency
    {
        private readonly ISettingManager _settingManager;
        private readonly ITimeZoneConverter _timeZoneConverter;

        public UserTimeZoneManager(ISettingManager settingManager, ITimeZoneConverter timeZoneConverter)
        {
            _settingManager = settingManager;
            _timeZoneConverter = timeZoneConverter;
        }

        /// <summary>
        /// Get the time zone id of given user.
        /// </summary>
        /// <returns>Time zone of the user. It is a windows time zone name.</returns>
        public Task<string> GetTimeZoneNameOfUserAsync(UserIdentifier userIdentifier)
        {
            return _settingManager.GetSettingValueForUserAsync(
                TimingSettingNames.TimeZone, userIdentifier);
        }

        /// <summary>
        /// Get the TimeZoneInfo of given user.
        /// </summary>
        public async Task<TimeZoneInfo> GetTimeZoneOfUserAsync(UserIdentifier userIdentifier)
        {
            return TimeZoneInfo.FindSystemTimeZoneById(await GetTimeZoneNameOfUserAsync(userIdentifier));
        }

        /// <summary>
        /// Set the time zone of given user.
        /// </summary>
        /// <param name="userIdentifier">The user identifier</param>
        /// <param name="timeZoneName">Time zone of the user. It is a windows time zone name.</param>
        public Task SetTimeZoneOfUserAsync(UserIdentifier userIdentifier, string timeZoneName)
        {
            return _settingManager.ChangeSettingForUserAsync(
                userIdentifier,
                TimingSettingNames.TimeZone,
                timeZoneName);
        }
    }
}
