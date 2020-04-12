namespace AcmStatisticsBackend.ServiceClients
{
    /// <summary>
    /// 爬虫的元数据
    /// </summary>
    public class CrawlerMetaResult
    {
        /// <summary>
        /// 爬虫名称（标识符）
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 爬虫标题（人类可读）
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 爬虫说明（可为空字符串）
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 爬虫 URL （可为空字符串）
        /// </summary>
        public string Url { get; set; }
    }
}
