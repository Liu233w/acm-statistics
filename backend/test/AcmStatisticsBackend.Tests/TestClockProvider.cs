using System;
using Abp.Timing;

namespace AcmStatisticsBackend.Tests
{
    internal class TestClockProvider : IClockProvider
    {
        public DateTime Normalize(DateTime dateTime)
        {
            return ClockProviders.Utc.Normalize(dateTime);
        }

        public DateTime Now { get; set; }

        public DateTimeKind Kind => DateTimeKind.Utc;

        public bool SupportsMultipleTimezone => true;
    }
}
