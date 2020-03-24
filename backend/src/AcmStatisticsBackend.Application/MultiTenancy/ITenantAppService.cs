using Abp.Application.Services;
using AcmStatisticsBackend.MultiTenancy.Dto;

namespace AcmStatisticsBackend.MultiTenancy
{
    public interface ITenantAppService : IAsyncCrudAppService<TenantDto, int, PagedTenantResultRequestDto, CreateTenantDto, TenantDto>
    {
    }
}
