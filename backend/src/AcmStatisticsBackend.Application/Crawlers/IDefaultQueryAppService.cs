using System.Threading.Tasks;
using Abp.Application.Services;
using AcmStatisticsBackend.Crawlers.Dto;

namespace AcmStatisticsBackend.Crawlers
{
    /// <summary>
    /// Manage user's default usernames which will be automatically entered in
    /// statistics page.
    /// </summary>
    public interface IDefaultQueryAppService : IApplicationService
    {
        /// <summary>
        /// Get user's default usernames which will be automatically entered in
        /// statistics page.
        /// </summary>
        Task<DefaultQueryDto> GetDefaultQueries();

        /// <summary>
        /// Set user's default usernames which will be automatically entered in
        /// statistics page.
        /// </summary>
        Task SetDefaultQueries(DefaultQueryDto dto);
    }
}
