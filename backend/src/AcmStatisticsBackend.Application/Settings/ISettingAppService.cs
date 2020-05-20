using System.Threading.Tasks;
using AcmStatisticsBackend.Settings.Dto;

namespace AcmStatisticsBackend.Settings
{
    public interface ISettingAppService
    {
        /// <summary>
        /// Get time zone of current user.
        /// </summary>
        Task<UserTimeZoneDto> GetUserTimeZone();

        /// <summary>
        /// Set time zone of current user.
        /// </summary>
        Task SetUserTimeZone(UserTimeZoneDto dto);
    }
}
