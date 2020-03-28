using System.Threading.Tasks;
using Abp.Application.Services;
using AcmStatisticsBackend.Authorization.Accounts.Dto;

namespace AcmStatisticsBackend.Authorization.Accounts
{
    public interface IAccountAppService : IApplicationService
    {
        Task<RegisterOutput> Register(RegisterInput input);
    }
}
