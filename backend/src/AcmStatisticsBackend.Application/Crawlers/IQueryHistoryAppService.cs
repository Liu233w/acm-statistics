using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using AcmStatisticsBackend.Crawlers.Dto;

namespace AcmStatisticsBackend.Crawlers
{
    /// <summary>
    /// Manage users' crawler query history
    /// </summary>
    public interface IQueryHistoryAppService : IApplicationService
    {
        /// <summary>
        /// Save a crawler query history, does not validate data.
        ///
        /// If there is another record in the same day, the old one is replaced.
        /// </summary>
        Task SaveOrReplaceQueryHistory(SaveOrReplaceQueryHistoryInput input);

        /// <summary>
        /// Delete a query history
        /// </summary>
        Task DeleteQueryHistory(DeleteQueryHistoryInput input);

        /// <summary>
        /// Get a list of current user's query history, sorted from newest to oldest.
        /// </summary>
        Task<PagedResultDto<GetQueryHistoryOutput>> GetQueryHistories(PagedResultRequestDto input);

        /// <summary>
        /// Get all <see cref="QueryWorkerHistory"/> that belong to certain <see cref="QueryHistory"/>.
        /// </summary>
        Task<ListResultDto<QueryWorkerHistoryDto>> GetQueryWorkerHistories(GetAcWorkerHistoryInput input);
    }
}
