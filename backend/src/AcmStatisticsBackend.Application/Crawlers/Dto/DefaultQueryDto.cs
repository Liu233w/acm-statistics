using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Abp.AutoMapper;
using Abp.Runtime.Validation;

namespace AcmStatisticsBackend.Crawlers.Dto
{
    /// <summary>
    /// 用户的默认爬虫用户名
    /// </summary>
    [AutoMap(typeof(DefaultQuery))]
    public class DefaultQueryDto : ICustomValidate
    {
        /// <summary>
        /// 主用户名
        /// </summary>
        /// [Required]
        [MinLength(0)]
        public string MainUsername { get; set; } = "";

        /// <summary>
        /// 在各个爬虫上的用户名。key为爬虫名称，value为一个用户名的列表，表示在该爬虫上的所有用户名。
        /// </summary>
        public Dictionary<string, List<string>> UsernamesInCrawlers { get; set; } = new Dictionary<string, List<string>>();

        public void AddValidationErrors(CustomValidationContext context)
        {
            foreach (var usernamesInCrawler in UsernamesInCrawlers)
            {
                if (usernamesInCrawler.Value == null)
                {
                    context.Results.Add(new ValidationResult("Items in UsernamesInCrawlers should not be null."));
                }
            }
        }
    }
}
