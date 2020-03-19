using Abp.Application.Services.Dto;

namespace AcmStatisticsBackend.Roles.Dto
{
    public class PagedRoleResultRequestDto : PagedResultRequestDto
    {
        public string Keyword { get; set; }
    }
}

