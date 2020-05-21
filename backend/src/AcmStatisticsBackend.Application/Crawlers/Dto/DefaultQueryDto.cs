using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Abp.AutoMapper;
using Abp.Runtime.Validation;

namespace AcmStatisticsBackend.Crawlers.Dto
{
    /// <summary>
    /// Store default query usernames
    /// </summary>
    [AutoMap(typeof(DefaultQuery))]
    public class DefaultQueryDto : ICustomValidate
    {
        /// <summary>
        /// main username
        /// </summary>
        [MinLength(0)]
        public string MainUsername { get; set; } = "";

        /// <summary>
        /// Usernames in each crawlers. Key is the name of crawler, value is a list that contains
        /// all usernames in this crawler.
        /// </summary>
        public Dictionary<string, List<string>> UsernamesInCrawlers { get; set; } =
            new Dictionary<string, List<string>>();

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
