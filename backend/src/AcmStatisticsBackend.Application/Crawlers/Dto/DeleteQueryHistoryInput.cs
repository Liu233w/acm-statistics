using System.Diagnostics.CodeAnalysis;

namespace AcmStatisticsBackend.Crawlers.Dto
{
    public class DeleteQueryHistoryInput
    {
        /// <summary>
        /// Delete history by certain id.
        /// </summary>
        public long? Id { get; set; }

        /// <summary>
        /// Delete histories in the list.
        /// </summary>
        [MaybeNull]
        public long[] Ids { get; set; }
    }
}
