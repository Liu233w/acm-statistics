using System.ComponentModel.DataAnnotations;
using System.Linq;
using Abp.Runtime.Validation;
using TimeZoneConverter;

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
            if (!TZConvert.KnownWindowsTimeZoneIds.Contains(TimeZone))
            {
                context.Results.Add(new ValidationResult("TimeZone must be valid!"));
            }
        }
    }
}
