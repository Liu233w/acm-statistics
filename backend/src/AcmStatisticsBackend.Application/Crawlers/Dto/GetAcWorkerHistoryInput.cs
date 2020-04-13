using System.ComponentModel.DataAnnotations;

namespace AcmStatisticsBackend.Crawlers.Dto
{
    public class GetAcWorkerHistoryInput
    {
        [Range(0, long.MaxValue)]
        public long AcHistoryId { get; set; }
    }
}
