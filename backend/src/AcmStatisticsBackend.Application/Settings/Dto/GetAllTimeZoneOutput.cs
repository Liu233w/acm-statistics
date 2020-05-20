using System.Collections.Generic;

namespace AcmStatisticsBackend.Settings.Dto
{
    public class GetAllTimeZoneOutput
    {
        /// <summary>
        /// A list of time zone names.
        /// See <see href="https://support.microsoft.com/en-au/help/973627/microsoft-time-zone-index-values" />
        /// for all possible values.
        /// </summary>
        public ICollection<string> TimeZoneList { get; set; }
    }
}
