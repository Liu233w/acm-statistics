using System.Threading.Tasks;
using Abp.Application.Services;
using AcmStatisticsBackend.Accounts.Dto;

namespace AcmStatisticsBackend.Accounts
{
    public interface IAccountAppService : IApplicationService
    {
        Task<RegisterOutput> Register(RegisterInput input);

        Task SelfDelete();
    }
}
