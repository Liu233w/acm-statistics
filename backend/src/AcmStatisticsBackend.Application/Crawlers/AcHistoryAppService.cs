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

        private readonly IRepository<AcWorkerHistory, long> _acWorkerHistoryRepository;

        public AcHistoryAppService(IRepository<AcHistory, long> acHistoryRepository,
            IRepository<AcWorkerHistory, long> acWorkerHistoryRepository)
        {
            _acHistoryRepository = acHistoryRepository;
            _acWorkerHistoryRepository = acWorkerHistoryRepository;
        }

        /// <inheritdoc cref="IAcHistoryAppService.SaveOrReplaceAcHistory"/>
        public async Task SaveOrReplaceAcHistory(SaveOrReplaceAcHistoryInput input)
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

        /// <inheritdoc cref="IAcHistoryAppService.GetAcHistory"/>
        public async Task<PagedResultDto<GetAcHistoryOutput>> GetAcHistory(PagedResultRequestDto input)
        {
            var list = await _acHistoryRepository.GetAll()
                .Where(e => e.UserId == AbpSession.UserId.Value)
                .OrderByDescending(e => e.CreationTime)
                .PageBy(input)
                .ToListAsync();

            var resultList = ObjectMapper.Map<List<GetAcHistoryOutput>>(list);
            return new PagedResultDto<GetAcHistoryOutput>(resultList.Count, resultList);
        }

        /// <inheritdoc cref="IAcHistoryAppService.GetAcWorkerHistory"/>
        public async Task<ListResultDto<AcWorkerHistoryDto>> GetAcWorkerHistory(GetAcWorkerHistoryInput input)
        {
            var entity = await GetAuthorizedEntity(input.AcHistoryId);
            // 目前abp的测试还不支持 LazyLoading，尽管实际程序里可以用，这里还是得手动加载
            // TODO: 等到abp更新之后看看这里还支不支持
            var entityList = await _acWorkerHistoryRepository.GetAll()
                .Where(e => e.AcHistoryId == entity.Id)
                .ToListAsync();
            var list = ObjectMapper.Map<List<AcWorkerHistoryDto>>(entityList);
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
                throw new AbpAuthorizationException("You do not have permissions to delete the entity.");
            }

            return acHistory;
        }

        /// <summary>
        /// 移除 AcHistory 和与其关联的 AcWorkerHistory，不检查用户是否有权限访问这个Entity
        /// </summary>
        private async Task DoDeleteHistory(AcHistory entity)
        {
            // 虽然MySql有Cascade删除，但是InMemoryDB还不支持这个。为了测试方便，这里手动删除一下。
            // TODO: 等到 https://github.com/dotnet/efcore/issues/3924 修复之后就把这里去掉
            await _acWorkerHistoryRepository.DeleteAsync(e => e.AcHistoryId == entity.Id);
            await _acHistoryRepository.DeleteAsync(entity);
        }
    }
}
