using System.Threading.Tasks;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Extensions;
using AcmStatisticsBackend.Authorization;
using AcmStatisticsBackend.Crawlers.Dto;

namespace AcmStatisticsBackend.Crawlers
{
    /// <inheritdoc cref="IDefaultQueryAppService"/>
    [AbpAuthorize(PermissionNames.Statistics_DefaultQuery)]
    public class DefaultQueryAppService : AcmStatisticsBackendAppServiceBase, IDefaultQueryAppService
    {
        private readonly IRepository<DefaultQuery, long> _defaultQueryRepository;

        public DefaultQueryAppService(IRepository<DefaultQuery, long> defaultQueryRepository)
        {
            _defaultQueryRepository = defaultQueryRepository;
        }

        /// <inheritdoc cref="IDefaultQueryAppService.GetDefaultQueries"/>
        public async Task<DefaultQueryDto> GetDefaultQueries()
        {
            var res = await _defaultQueryRepository.FirstOrDefaultAsync(e => e.UserId == AbpSession.UserId.Value);
            return res == null ? new DefaultQueryDto() : ObjectMapper.Map<DefaultQueryDto>(res);
        }

        /// <inheritdoc cref="IDefaultQueryAppService.SetDefaultQueries"/>
        public async Task SetDefaultQueries(DefaultQueryDto dto)
        {
            var entity = ObjectMapper.Map<DefaultQuery>(dto);

            // 移除空的用户名
            foreach (var (crawlerName, userNames) in entity.UsernamesInCrawlers)
            {
                userNames.RemoveAll(s => s.IsNullOrEmpty());
                if (userNames.Count == 0)
                {
                    entity.UsernamesInCrawlers.Remove(crawlerName);
                }
            }

            var userId = AbpSession.UserId.Value;
            var existEntity = await _defaultQueryRepository.FirstOrDefaultAsync(e => e.UserId == userId);
            if (existEntity == null)
            {
                entity.UserId = userId;
                await _defaultQueryRepository.InsertAsync(entity);
            }
            else
            {
                existEntity.MainUsername = entity.MainUsername;
                existEntity.UsernamesInCrawlers = entity.UsernamesInCrawlers;
            }
        }
    }
}
