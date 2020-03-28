using Abp.Application.Services.Dto;

namespace AcmStatisticsBackend.Users.Dto
{
    // custom PagedResultRequestDto
    public class PagedUserResultRequestDto : PagedResultRequestDto
    {
        public string Keyword { get; set; }
        public bool? IsActive { get; set; }
    }
}
