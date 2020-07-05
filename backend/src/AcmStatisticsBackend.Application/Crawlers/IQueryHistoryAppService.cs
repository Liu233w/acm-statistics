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
        /// <returns>The new query history id</returns>
        Task<SaveOrReplaceQueryHistoryOutput> SaveOrReplaceQueryHistory(SaveOrReplaceQueryHistoryInput input);

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

        /// <summary>
        /// Get query summary of certain query history
        /// </summary>
        /// <param name="input">input the id of query history</param>
        Task<QuerySummaryDto> GetQuerySummary(GetQuerySummaryInput input);

        /// <summary>
        /// Get a list of current user's query history, sorted from newest to oldest.
        /// The submission and solved number are also included.
        /// </summary>
        Task<PagedResultDto<GetQueryHistoryAndSummaryOutput>>
            GetQueryHistoriesAndSummaries(PagedResultRequestDto input);
    }
}
