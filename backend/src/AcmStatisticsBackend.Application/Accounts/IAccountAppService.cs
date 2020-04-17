using System.Threading.Tasks;
using Abp.Application.Services;
using AcmStatisticsBackend.Accounts.Dto;

namespace AcmStatisticsBackend.Accounts
{
    public interface IAccountAppService : IApplicationService
    {
        Task<RegisterOutput> Register(RegisterInput input);

        /// <summary>
        /// Delete this account
        /// </summary>
        Task SelfDelete();

        /// <summary>
        /// Change password of current user
        /// </summary>
        Task ChangePassword(ChangePasswordInput input);
    }
}
