using System.Threading.Tasks;
using Abp.Application.Services;
using AcmStatisticsBackend.Sessions.Dto;

namespace AcmStatisticsBackend.Sessions
{
    public interface ISessionAppService : IApplicationService
    {
        Task<GetCurrentLoginInformationsOutput> GetCurrentLoginInformations();
    }
}
