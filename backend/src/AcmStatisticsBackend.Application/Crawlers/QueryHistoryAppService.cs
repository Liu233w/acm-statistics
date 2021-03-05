using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Linq.Extensions;
using Abp.Timing;
using Abp.Timing.Timezone;
using Abp.UI;
using AcmStatisticsBackend.Authorization;
using AcmStatisticsBackend.Crawlers.Dto;
using AcmStatisticsBackend.ServiceClients;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AcmStatisticsBackend.Crawlers
{
    /// <inheritdoc cref="IQueryHistoryAppService"/>
    [AbpAuthorize(PermissionNames.AcHistory_Histories)]
    public class QueryHistoryAppService : AcmStatisticsBackendAppServiceBase, IQueryHistoryAppService
    {
        private readonly IRepository<QueryHistory, long> _acHistoryRepository;
        private readonly IRepository<QueryWorkerHistory, long> _acWorkerHistoryRepository;
        private readonly IRepository<QuerySummary, long> _querySummaryRepository;
        private readonly IRepository<QueryCrawlerSummary, long> _queryCrawlerSummaryRepository;
        private readonly IClockProvider _clockProvider;
        private readonly ITimeZoneConverter _timeZoneConverter;
        private readonly ICrawlerApiBackendClient _crawlerApiBackendClient;
        private readonly SummaryGenerator _summaryGenerator;

        public QueryHistoryAppService(
            IRepository<QueryHistory, long> acHistoryRepository,
            IRepository<QueryWorkerHistory, long> acWorkerHistoryRepository,
            IClockProvider clockProvider,
            ITimeZoneConverter timeZoneConverter,
            ICrawlerApiBackendClient crawlerApiBackendClient,
            IRepository<QuerySummary, long> querySummaryRepository,
            IRepository<QueryCrawlerSummary, long> queryCrawlerSummaryRepository,
            SummaryGenerator summaryGenerator)
        {
            _acHistoryRepository = acHistoryRepository;
            _acWorkerHistoryRepository = acWorkerHistoryRepository;
            _clockProvider = clockProvider;
            _timeZoneConverter = timeZoneConverter;
            _crawlerApiBackendClient = crawlerApiBackendClient;
            _querySummaryRepository = querySummaryRepository;
            _queryCrawlerSummaryRepository = queryCrawlerSummaryRepository;
            _summaryGenerator = summaryGenerator;
        }

        /// <inheritdoc cref="IQueryHistoryAppService.SaveOrReplaceQueryHistory"/>
        public async Task<SaveOrReplaceQueryHistoryOutput> SaveOrReplaceQueryHistory(
            SaveOrReplaceQueryHistoryInput input)
        {
            // 添加新记录
            var acHistory = ObjectMapper.Map<QueryHistory>(input);

            // AutoMapper will change empty array to null. The code below is used to restore them
            foreach (var (entity, dto) in acHistory.QueryWorkerHistories.Zip(input.QueryWorkerHistories))
            {
                if (dto.SolvedList == null)
                {
                    entity.SolvedList = null;
                }

                if (dto.SubmissionsByCrawlerName == null)
                {
                    entity.SubmissionsByCrawlerName = null;
                }
            }

            Debug.Assert(AbpSession.UserId != null, "AbpSession.UserId != null");
            acHistory.UserId = AbpSession.UserId.Value;
            acHistory.CreationTime = _clockProvider.Now;
            acHistory.IsReliableSource = false;

            var crawlerMeta = await _crawlerApiBackendClient.GetCrawlerMeta();
            var querySummary = _summaryGenerator.Generate(
                crawlerMeta,
                acHistory.QueryWorkerHistories.AsReadOnly());

            await RemoveLatestHistoryTheSameDayOf(acHistory.CreationTime);

            var historyId = await _acHistoryRepository.InsertAndGetIdAsync(acHistory);

            querySummary.QueryHistoryId = historyId;
            await _querySummaryRepository.InsertAsync(querySummary);

            return new SaveOrReplaceQueryHistoryOutput
            {
                QueryHistoryId = historyId,
            };
        }

        private async Task RemoveLatestHistoryTheSameDayOf(DateTime day)
        {
            var latestItem = await _acHistoryRepository.GetAll()
                .Where(e => e.UserId == AbpSession.UserId.Value)
                .OrderByDescending(e => e.CreationTime)
                .FirstOrDefaultAsync();
            if (latestItem != null)
            {
                Debug.Assert(AbpSession.UserId != null, "AbpSession.UserId != null");
                var currentLocalTime = _timeZoneConverter.Convert(
                    day, AbpSession.TenantId, AbpSession.UserId.Value);
                var latestItemCreationTimeLocal = _timeZoneConverter.Convert(
                    latestItem.CreationTime, AbpSession.TenantId, AbpSession.UserId.Value);

                Debug.Assert(currentLocalTime != null, nameof(currentLocalTime) + " != null");
                Debug.Assert(latestItemCreationTimeLocal != null, nameof(latestItemCreationTimeLocal) + " != null");
                if (latestItemCreationTimeLocal.Value.Date == currentLocalTime.Value.Date)
                {
                    await DoDeleteHistory(latestItem);
                }
            }
        }

        /// <inheritdoc cref="IQueryHistoryAppService.DeleteQueryHistory"/>
        [HttpPost]
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
            var list = await QueryHistoriesOfCurrentUser()
                .OrderByDescending(e => e.CreationTime)
                .PageBy(input)
                .ToListAsync();
            var count = await QueryHistoriesOfCurrentUser().CountAsync();

            var resultList = ObjectMapper.Map<List<GetQueryHistoryOutput>>(list);
            return new PagedResultDto<GetQueryHistoryOutput>(count, resultList);
        }

        private IQueryable<QueryHistory> QueryHistoriesOfCurrentUser()
        {
            return _acHistoryRepository.GetAll()
                .Where(e => e.UserId == AbpSession.UserId.Value);
        }

        /// <inheritdoc cref="IQueryHistoryAppService.GetQueryWorkerHistories"/>
        public async Task<ListResultDto<QueryWorkerHistoryDto>> GetQueryWorkerHistories(GetAcWorkerHistoryInput input)
        {
            var queryHistory = await GetAuthorizedEntity(input.QueryHistoryId);
            var entityList = await _acWorkerHistoryRepository.GetAll()
                .Where(e => e.QueryHistoryId == queryHistory.Id)
                .ToListAsync();
            var list = ObjectMapper.Map<List<QueryWorkerHistoryDto>>(entityList);

            // AutoMapper will change empty array to null. The code below is used to restore them
            foreach (var (dto, entity) in list.Zip(entityList))
            {
                if (entity.SolvedList == null)
                {
                    dto.SolvedList = null;
                }

                if (entity.SubmissionsByCrawlerName == null)
                {
                    dto.SubmissionsByCrawlerName = null;
                }
            }

            return new ListResultDto<QueryWorkerHistoryDto>(list);
        }

        /// <inheritdoc cref="IQueryHistoryAppService.GetQuerySummary" />
        public async Task<QuerySummaryDto> GetQuerySummary(GetQuerySummaryInput input)
        {
            var history = await GetAuthorizedEntity(input.QueryHistoryId);

            var summary = await _querySummaryRepository.FirstOrDefaultAsync(
                e => e.QueryHistoryId == history.Id);

            if (summary == null)
            {
                throw new UserFriendlyException("This query history does not have summary");
            }

            var queryCrawlerSummary = await _queryCrawlerSummaryRepository.GetAllIncluding(
                    e => e.Usernames)
                .Where(e => e.QuerySummaryId == summary.Id)
                .ToListAsync();

            var querySummaryDto = ObjectMapper.Map<QuerySummaryDto>(summary);
            querySummaryDto.QueryCrawlerSummaries =
                ObjectMapper.Map<ICollection<QueryCrawlerSummaryDto>>(queryCrawlerSummary);
            querySummaryDto.MainUsername = history.MainUsername;

            return querySummaryDto;
        }

        /// <inheritdoc cref="IQueryHistoryAppService.GetQueryHistoriesAndSummaries" />
        public async Task<PagedResultDto<GetQueryHistoryAndSummaryOutput>> GetQueryHistoriesAndSummaries(
            PagedResultRequestDto input)
        {
            var list = await
                (from h in QueryHistoriesOfCurrentUser()
                    .OrderByDescending(e => e.CreationTime)
                    .PageBy(input)
                 join s in _querySummaryRepository.GetAll()
                     on h.Id equals s.Id into grouping
                 from s in grouping.DefaultIfEmpty()
                 select new GetQueryHistoryAndSummaryOutput
                 {
                     HistoryId = h.Id,
                     SummaryId = s.Id,
                     CreationTime = h.CreationTime,
                     Solved = s.Solved == 0 && s.Submission == 0 ? null : s.Solved,
                     Submission = s.Solved == 0 && s.Submission == 0 ? null : s.Submission,
                 }).ToListAsync();
            var count = await QueryHistoriesOfCurrentUser().CountAsync();

            return new PagedResultDto<GetQueryHistoryAndSummaryOutput>(count, list);
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
                throw new AbpAuthorizationException("You do not have permissions to visit the entity.");
            }

            return acHistory;
        }

        /// <summary>
        /// 移除 AcHistory 和与其关联的 AcWorkerHistory，不检查用户是否有权限访问这个Entity
        /// </summary>
        private async Task DoDeleteHistory(QueryHistory entity)
        {
            await _acHistoryRepository.DeleteAsync(entity);
        }
    }
}
