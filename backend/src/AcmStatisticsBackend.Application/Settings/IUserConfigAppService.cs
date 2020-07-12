using System.Threading.Tasks;
using AcmStatisticsBackend.Settings.Dto;

namespace AcmStatisticsBackend.Settings
{
    /// <summary>
    /// Manage user config
    /// </summary>
    public interface IUserConfigAppService
    {
        /// <summary>
        /// Get all user settings available to frontend
        /// </summary>
        Task<UserSettingsConfigDto> GetUserSettings();

        /// <summary>
        /// Update config about whether the history should be auto-saved
        /// </summary>
        Task UpdateAutoSaveHistory(UpdateAutoSaveHistoryInput input);

        /// <summary>
        /// Set time zone of current user.
        /// </summary>
        Task SetUserTimeZone(UserTimeZoneDto dto);
    }
}
