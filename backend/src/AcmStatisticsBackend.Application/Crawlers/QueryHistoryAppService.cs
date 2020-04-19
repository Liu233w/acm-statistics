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
    /// <inheritdoc cref="IQueryHistoryAppService"/>
    [AbpAuthorize(PermissionNames.AcHistory_Histories)]
    public class QueryHistoryAppService : AcmStatisticsBackendAppServiceBase, IQueryHistoryAppService
    {
        private readonly IRepository<QueryHistory, long> _acHistoryRepository;

        private readonly IRepository<QueryWorkerHistory, long> _acWorkerHistoryRepository;

        public QueryHistoryAppService(IRepository<QueryHistory, long> acHistoryRepository,
            IRepository<QueryWorkerHistory, long> acWorkerHistoryRepository)
        {
            _acHistoryRepository = acHistoryRepository;
            _acWorkerHistoryRepository = acWorkerHistoryRepository;
        }

        /// <inheritdoc cref="IQueryHistoryAppService.SaveOrReplaceQueryHistory"/>
        public async Task SaveOrReplaceQueryHistory(SaveOrReplaceQueryHistoryInput input)
        {
            // 移除同一天的记录
            // TODO: 目前是UTC时间。可以改成用户的时区。
            var latestItem = await _acHistoryRepository.GetAll()
                .Where(e => e.UserId == AbpSession.UserId.Value)
                .OrderByDescending(e => e.CreationTime)
                .FirstOrDefaultAsync();
            if (latestItem != null && latestItem.CreationTime.Date == Clock.Now.Date)
            {
                await DoDeleteHistory(latestItem);
            }

            // 添加新记录
            var acHistory = ObjectMapper.Map<QueryHistory>(input);
            Debug.Assert(AbpSession.UserId != null, "AbpSession.UserId != null");
            acHistory.UserId = AbpSession.UserId.Value;
            // 会自动添加关联的 AcWorkerHistory
            await _acHistoryRepository.InsertAsync(acHistory);
        }

        /// <inheritdoc cref="IQueryHistoryAppService.DeleteQueryHistory"/>
        public async Task DeleteQueryHistory(DeleteQueryHistoryInput input)
        {
            if (input.Id.HasValue)
            {
                var entity = await GetAuthorizedEntity(input.Id.Value);
                await DoDeleteHistory(entity);
            }

            if (input.Ids != null)
            {
                foreach (var id in input.Ids)
                {
                    var entity = await GetAuthorizedEntity(id);
                    await DoDeleteHistory(entity);
                }
            }
        }

        /// <inheritdoc cref="IQueryHistoryAppService.GetQueryHistories"/>
        public async Task<PagedResultDto<GetQueryHistoryOutput>> GetQueryHistories(PagedResultRequestDto input)
        {
            var list = await _acHistoryRepository.GetAll()
                .Where(e => e.UserId == AbpSession.UserId.Value)
                .OrderByDescending(e => e.CreationTime)
                .PageBy(input)
                .ToListAsync();

            var resultList = ObjectMapper.Map<List<GetQueryHistoryOutput>>(list);
            return new PagedResultDto<GetQueryHistoryOutput>(resultList.Count, resultList);
        }

        /// <inheritdoc cref="IQueryHistoryAppService.GetQueryWorkerHistories"/>
        public async Task<ListResultDto<QueryWorkerHistoryDto>> GetQueryWorkerHistories(GetAcWorkerHistoryInput input)
        {
            var entity = await GetAuthorizedEntity(input.QueryHistoryId);
            var entityList = await _acWorkerHistoryRepository.GetAll()
                .Where(e => e.QueryHistoryId == entity.Id)
                .ToListAsync();
            var list = ObjectMapper.Map<List<QueryWorkerHistoryDto>>(entityList);
            return new ListResultDto<QueryWorkerHistoryDto>(list);
        }

        /// <summary>
        /// 根据ID获取对象，并检查权限。
        /// </summary>
        /// <param name="id">AcHistory的ID</param>
        /// <returns>AcHistory</returns>
        /// <exception cref="AbpAuthorizationException">如果该对象不是由此用户创建，抛出异常</exception>
        private async Task<QueryHistory> GetAuthorizedEntity(long id)
        {
            var acHistory = await _acHistoryRepository.GetAsync(id);
            if (acHistory.UserId != AbpSession.UserId)
            {
                throw new AbpAuthorizationException("You do not have permissions to delete the entity.");
            }

            return acHistory;
        }

        /// <summary>
        /// 移除 AcHistory 和与其关联的 AcWorkerHistory，不检查用户是否有权限访问这个Entity
        /// </summary>
        private async Task DoDeleteHistory(QueryHistory entity)
        {
            // 虽然MySql有Cascade删除，但是InMemoryDB还不支持这个。为了测试方便，这里手动删除一下。
            // TODO: 等到 https://github.com/dotnet/efcore/issues/3924 修复之后就把这里去掉
            await _acWorkerHistoryRepository.DeleteAsync(e => e.QueryHistoryId == entity.Id);
            await _acHistoryRepository.DeleteAsync(entity);
        }
    }
}
