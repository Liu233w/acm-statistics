using System;
using Abp.Timing;

namespace AcmStatisticsBackend.Tests
{
    /// <summary>
    /// Used to mock time. When setting Now, its Kind will be set to UTC.
    /// </summary>
    public class TestClockProvider : IClockProvider
    {
        private DateTime _now;

        public DateTime Normalize(DateTime dateTime)
        {
            return ClockProviders.Utc.Normalize(dateTime);
        }

        public DateTime Now
        {
            get => _now;
            set => _now = DateTime.SpecifyKind(value, DateTimeKind.Utc);
        }

        public DateTimeKind Kind => DateTimeKind.Utc;

        public bool SupportsMultipleTimezone => true;
    }
}
