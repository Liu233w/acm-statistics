using System.Threading.Tasks;
using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;
using AcmStatisticsBackend.Crawlers.Dto;

namespace AcmStatisticsBackend.Crawlers
{
    public class AcHistoryAppService : AcmStatisticsBackendAppServiceBase, IAcHistoryAppService
    {
        private readonly IRepository<AcHistory, long> _acHistoryRepository;

        private readonly IRepository<AcWorkerHistory, long> _acWorkerHistoryRepository;
        
        private readonly IRepo
        
        public async Task SaveOrReplaceAcHistory(AcHistoryDto dto)
        {
            throw new System.NotImplementedException();
        }

        public async Task DeleteAcHistory(DeleteAcHistoryInput input)
        {
            throw new System.NotImplementedException();
        }

        public async Task<PagedResultDto<AcHistoryDto>> GetAcHistory(GetAcHistoryInput input)
        {
            throw new System.NotImplementedException();
        }

        public async Task<ListResultDto<AcWorkerHistoryDto>> GetAcWorkerHistory(GetAcWorkerHistoryInput input)
        {
            throw new System.NotImplementedException();
        }
    }
}
