using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using AcmStatisticsBackend.Roles.Dto;
using AcmStatisticsBackend.Users.Dto;

namespace AcmStatisticsBackend.Users
{
    public interface IUserAppService : IAsyncCrudAppService<UserDto, long, PagedUserResultRequestDto, CreateUserDto, UserDto>
    {
        Task<ListResultDto<RoleDto>> GetRoles();

        Task ChangeLanguage(ChangeUserLanguageDto input);
    }
}
