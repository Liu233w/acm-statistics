using System;
using System.ComponentModel.DataAnnotations;
using Abp.Runtime.Validation;

namespace AcmStatisticsBackend.Settings.Dto
{
    public class UserTimeZoneDto : ICustomValidate
    {
        /// <summary>
        /// Time zone of the user. It is a windows time zone name.
        /// See <see href="https://support.microsoft.com/en-au/help/973627/microsoft-time-zone-index-values" />
        /// for all possible values.
        /// </summary>
        [Required]
        public string TimeZone { get; set; }

        public void AddValidationErrors(CustomValidationContext context)
        {
            try
            {
                TimeZoneInfo.FindSystemTimeZoneById(TimeZone);
            }
            catch (TimeZoneNotFoundException)
            {
                context.Results.Add(new ValidationResult("TimeZone must be valid!"));
            }
        }
    }
}
