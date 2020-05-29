using System;
using System.Threading.Tasks;
using Abp.Runtime.Validation;
using Abp.UI;
using AcmStatisticsBackend.Settings;
using AcmStatisticsBackend.Settings.Dto;
using FluentAssertions;
using Xunit;

namespace AcmStatisticsBackend.Tests.Settings
{
    public class TimeZoneSettingAppService_Tests : AcmStatisticsBackendTestBase
    {
        private readonly ITimeZoneSettingAppService _timeZoneSettingAppService;
        private readonly TestClockProvider _clockProvider;

        public TimeZoneSettingAppService_Tests()
        {
            _clockProvider = new TestClockProvider();
            _timeZoneSettingAppService = Resolve<TimeZoneSettingAppService>(new
            {
                clockProvider = _clockProvider,
            });
        }

        [Fact]
        public async Task GetUserTimeZone_ShouldWork()
        {
            // arrange
            LoginAsHostAdmin();
            await _timeZoneSettingAppService.SetUserTimeZone(new UserTimeZoneDto
            {
                TimeZone = "US Eastern Standard Time",
            });

            // act
            var result = await _timeZoneSettingAppService.GetUserTimeZone();

            // assert
            result.Should().BeEquivalentTo(new UserTimeZoneDto
            {
                TimeZone = "US Eastern Standard Time",
            });
        }

        [Fact]
        public async Task GetUserTimeZone_WhenNoTimeZoneSet_ShouldReturnDefaultTimeZone()
        {
            // arrange
            LoginAsHostAdmin();

            // act
            var result = await _timeZoneSettingAppService.GetUserTimeZone();

            // assert
            result.Should().BeEquivalentTo(new UserTimeZoneDto
            {
                TimeZone = "UTC",
            });
        }

        [Fact]
        public async Task SetUserTimeZone_WhenTimeZoneIsInvalid_ShouldThrow()
        {
            LoginAsHostAdmin();

            await _timeZoneSettingAppService.SetUserTimeZone(new UserTimeZoneDto
            {
                TimeZone = "BlaBla",
            }).ShouldThrow<AbpValidationException>();
        }

        [Fact]
        public async Task SetUserTimeZone_ShouldWork()
        {
            // arrange
            LoginAsHostAdmin();

            // act
            await _timeZoneSettingAppService.SetUserTimeZone(new UserTimeZoneDto
            {
                TimeZone = "US Eastern Standard Time",
            });

            // assert
            var timeZoneOfUser = await _timeZoneSettingAppService.GetUserTimeZone();
            timeZoneOfUser.TimeZone.Should().Be("US Eastern Standard Time");
        }

        [Fact]
        public async Task SetUserTimeZone_CannotBeSetTwiceWithIn24Hours()
        {
            _clockProvider.Now = new DateTime(2020, 4, 1, 0, 0, 0);

            await _timeZoneSettingAppService.SetUserTimeZone(new UserTimeZoneDto
            {
                // GMT +01:00
                TimeZone = "Central Europe Standard Time",
            });

            // UTC: 23:00 April 1; Local: 00:00 April 2
            _clockProvider.Now = _clockProvider.Now.AddHours(23);

            // should not set time zone
            await _timeZoneSettingAppService.SetUserTimeZone(new UserTimeZoneDto
            {
                TimeZone = "Dateline Standard Time",
            }).ShouldThrow<UserFriendlyException>();

            // UTC: 00:00 April 2; Local: 01:00 April 2
            _clockProvider.Now = _clockProvider.Now.AddHours(1);

            // Can set time zone
            await _timeZoneSettingAppService.SetUserTimeZone(new UserTimeZoneDto
            {
                TimeZone = "Dateline Standard Time",
            });

            var timeZoneOfUser = await _timeZoneSettingAppService.GetUserTimeZone();
            timeZoneOfUser.TimeZone.Should().Be("Dateline Standard Time");
        }

        [Fact]
        public async Task SetUserTimeZone_WhenInputWithNonWindowsIds_ShouldThrow()
        {
            // arrange
            LoginAsHostAdmin();

            // act
            var task = _timeZoneSettingAppService.SetUserTimeZone(new UserTimeZoneDto
            {
                // Iana time zone id
                TimeZone = "America/Santiago",
            });

            // assert
            await task.ShouldThrow<AbpValidationException>();
        }
    }
}
