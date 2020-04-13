using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using AcmStatisticsBackend.Crawlers.Dto;

namespace AcmStatisticsBackend.Crawlers
{
    /// <summary>
    /// 管理用户的 AC 历史记录
    /// </summary>
    public interface IAcHistoryAppService : IApplicationService
    {
        /// <summary>
        /// 保存当前用户的一个查询历史记录。不负责验证数据是否正确。
        ///
        /// 如果在同一天内有另一个查询，将删除旧的查询。
        /// </summary>
        Task SaveOrReplaceAcHistory(SaveOrReplaceAcHistoryInput input);

        /// <summary>
        /// 删除历史记录
        /// </summary>
        Task DeleteAcHistory(DeleteAcHistoryInput input);

        /// <summary>
        /// 获取当前用户的AC历史记录列表，从新到旧排序。
        /// </summary>
        Task<PagedResultDto<GetAcHistoryOutput>> GetAcHistory(PagedResultRequestDto input);

        /// <summary>
        /// 获取属于某一个 <see cref="AcHistory"/> 的所有 <see cref="AcWorkerHistory"/>
        /// </summary>
        Task<ListResultDto<AcWorkerHistoryDto>> GetAcWorkerHistory(GetAcWorkerHistoryInput input);
    }
}
