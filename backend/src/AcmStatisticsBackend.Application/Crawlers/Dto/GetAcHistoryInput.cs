using Abp.Application.Services.Dto;

namespace AcmStatisticsBackend.Crawlers.Dto
{
    public class GetAcHistoryInput : IPagedResultRequest
    {
        public int MaxResultCount { get; set; }

        public int SkipCount { get; set; }
    }
}
