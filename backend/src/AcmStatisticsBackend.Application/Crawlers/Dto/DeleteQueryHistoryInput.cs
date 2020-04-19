using System.Diagnostics.CodeAnalysis;

namespace AcmStatisticsBackend.Crawlers.Dto
{
    public class DeleteQueryHistoryInput
    {
        /// <summary>
        /// 根据ID来删除历史记录
        /// </summary>
        public long? Id { get; set; }

        /// <summary>
        /// 删除一系列历史记录
        /// </summary>
        [MaybeNull]
        public long[] Ids { get; set; }
    }
}
