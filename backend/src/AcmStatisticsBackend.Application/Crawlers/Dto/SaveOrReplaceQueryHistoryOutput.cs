using System.ComponentModel.DataAnnotations;

namespace AcmStatisticsBackend.Crawlers.Dto
{
    public class SaveOrReplaceQueryHistoryOutput
    {
        [Range(1, long.MaxValue)]
        public long QueryHistoryId { get; set; }
    }
}
