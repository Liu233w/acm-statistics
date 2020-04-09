using Abp.Authorization;
using AcmStatisticsBackend.Authorization;
using AcmStatisticsBackend.Crawlers.Dto;

namespace AcmStatisticsBackend.Crawlers
{
    /// <inheritdoc cref="IDefaultQueryAppService"/>
    [AbpAuthorize(PermissionNames.Statistics_DefaultQuery)]
    public class DefaultQueryAppService : AcmStatisticsBackendAppServiceBase, IDefaultQueryAppService
    {
        /// <inheritdoc cref="IDefaultQueryAppService.GetDefaultQueries"/>
        public DefaultQueryDto GetDefaultQueries()
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc cref="IDefaultQueryAppService.SetDefaultQueries"/>
        public void SetDefaultQueries(DefaultQueryDto dto)
        {
            throw new System.NotImplementedException();
        }
    }
}