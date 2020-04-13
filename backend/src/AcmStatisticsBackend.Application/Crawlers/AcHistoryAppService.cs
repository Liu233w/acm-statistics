using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Linq.Extensions;
using Abp.Timing;
using AcmStatisticsBackend.Authorization;
using AcmStatisticsBackend.Crawlers.Dto;
using Microsoft.EntityFrameworkCore;

namespace AcmStatisticsBackend.Crawlers
{
    /// <inheritdoc cref="IAcHistoryAppService"/>
    [AbpAuthorize(PermissionNames.AcHistory_Histories)]
    public class AcHistoryAppService : AcmStatisticsBackendAppServiceBase, IAcHistoryAppService
    {
        private readonly IRepository<AcHistory, long> _acHistoryRepository;

        public AcHistoryAppService(IRepository<AcHistory, long> acHistoryRepository)
        {
            _acHistoryRepository = acHistoryRepository;
        }

        /// <inheritdoc cref="IAcHistoryAppService.SaveOrReplaceAcHistory"/>
        public async Task SaveOrReplaceAcHistory(SaveOrReplaceAcHistoryInput input)
        {
            // 移除同一天的记录
            // TODO: 目前是UTC时间。可以改成用户的时区。
            var latestItem = await _acHistoryRepository.GetAll()
                .Where(e => e.UserId == AbpSession.UserId.Value)
                .OrderBy(e => e.CreationTime)
                .FirstOrDefaultAsync();
            if (latestItem.CreationTime.Date == Clock.Now.Date)
            {
                await _acHistoryRepository.DeleteAsync(latestItem);
            }

            // 添加新记录
            var acHistory = ObjectMapper.Map<AcHistory>(input);
            Debug.Assert(AbpSession.UserId != null, "AbpSession.UserId != null");
            acHistory.UserId = AbpSession.UserId.Value;
            // 会自动添加关联的 AcWorkerHistory
            await _acHistoryRepository.InsertAsync(acHistory);
        }

        /// <inheritdoc cref="IAcHistoryAppService.DeleteAcHistory"/>
        public async Task DeleteAcHistory(DeleteAcHistoryInput input)
        {
            if (input.Id.HasValue)
            {
                var entity = await GetAuthorizedEntity(input.Id.Value);
                // 关联的 AcWorkerHistory 会自动删除
                await _acHistoryRepository.DeleteAsync(entity);
            }

            foreach (var id in input.Ids)
            {
                var entity = await GetAuthorizedEntity(id);
                // 关联的 AcWorkerHistory 会自动删除
                await _acHistoryRepository.DeleteAsync(entity);
            }
        }

        /// <inheritdoc cref="IAcHistoryAppService.GetAcHistory"/>
        public async Task<PagedResultDto<GetAcHistoryOutput>> GetAcHistory(PagedResultRequestDto input)
        {
            var list = await _acHistoryRepository.GetAll()
                .Where(e => e.UserId == AbpSession.UserId.Value)
                .OrderBy(e => e.CreationTime)
                .PageBy(input)
                .ToListAsync();

            var resultList = ObjectMapper.Map<List<GetAcHistoryOutput>>(list);
            return new PagedResultDto<GetAcHistoryOutput>(resultList.Count, resultList);
        }

        /// <inheritdoc cref="IAcHistoryAppService.GetAcWorkerHistory"/>
        public async Task<ListResultDto<AcWorkerHistoryDto>> GetAcWorkerHistory(GetAcWorkerHistoryInput input)
        {
            var entity = await GetAuthorizedEntity(input.AcHistoryId);
            var list = ObjectMapper.Map<List<AcWorkerHistoryDto>>(entity.AcWorkerHistories);
            return new ListResultDto<AcWorkerHistoryDto>(list);
        }

        /// <summary>
        /// 根据ID获取对象，并检查权限。
        /// </summary>
        /// <param name="id">AcHistory的ID</param>
        /// <returns>AcHistory</returns>
        /// <exception cref="AbpAuthorizationException">如果该对象不是由此用户创建，抛出异常</exception>
        private async Task<AcHistory> GetAuthorizedEntity(long id)
        {
            var acHistory = await _acHistoryRepository.GetAsync(id);
            if (acHistory.UserId != AbpSession.UserId)
            {
                throw new AbpAuthorizationException("您没有权限处理此数据");
            }

            return acHistory;
        }
    }
}
