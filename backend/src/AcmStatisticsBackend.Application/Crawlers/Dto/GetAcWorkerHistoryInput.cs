using System.ComponentModel.DataAnnotations;

namespace AcmStatisticsBackend.Crawlers.Dto
{
    public class GetAcWorkerHistoryInput
    {
        [Range(0, long.MaxValue)]
        public long QueryHistoryId { get; set; }
    }
}
