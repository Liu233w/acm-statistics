using System;
using System.Threading.Tasks;
using Abp.Runtime.Validation;
using Abp.Timing;
using Abp.UI;
using AcmStatisticsBackend.Settings;
using AcmStatisticsBackend.Settings.Dto;
using AcmStatisticsBackend.Timing;
using Shouldly;
using Xunit;

namespace AcmStatisticsBackend.Tests.Settings
{
    public class SettingAppService_Tests : AcmStatisticsBackendTestBase
    {
        private readonly ISettingAppService _settingAppService;
        private readonly UserTimeZoneManager _userTimeZoneManager;
        private readonly TestClockProvider _clockProvider;

        public SettingAppService_Tests()
        {
            _clockProvider = new TestClockProvider();
            _settingAppService = Resolve<SettingAppService>(new
            {
                clockProvider = _clockProvider,
            });
            _userTimeZoneManager = Resolve<UserTimeZoneManager>();
        }

        [Fact]
        public async Task GetUserTimeZone_ShouldWork()
        {
            // arrange
            await UsingDbContextAsync(async ctx =>
            {
                await _userTimeZoneManager.SetTimeZoneOfUserAsync(
                    GetHostAdmin().ToUserIdentifier(), "US Eastern Standard Time");
            });

            LoginAsHostAdmin();

            // act
            var result = await _settingAppService.GetUserTimeZone();

            // assert
            result.ShouldEqualInJson(new UserTimeZoneDto
            {
                TimeZone = "US Eastern Standard Time",
            });
        }

        [Fact]
        public async Task SetUserTimeZone_ShouldThrowWhenTimeZoneIsInvalid()
        {
            LoginAsHostAdmin();

            await _settingAppService.SetUserTimeZone(new UserTimeZoneDto
            {
                TimeZone = "BlaBla",
            }).ShouldThrowAsync<AbpValidationException>();
        }

        [Fact]
        public async Task SetUserTimeZone_ShouldWork()
        {
            // arrange
            LoginAsHostAdmin();

            // act
            await _settingAppService.SetUserTimeZone(new UserTimeZoneDto
            {
                TimeZone = "US Eastern Standard Time",
            });

            // assert
            await UsingDbContextAsync(async ctx =>
            {
                var timeZoneOfUser =
                    await _userTimeZoneManager.GetTimeZoneOfUserAsync(GetHostAdmin().ToUserIdentifier());

                timeZoneOfUser.Id.ShouldBe("US Eastern Standard Time");
            });
        }

        [Fact]
        public async Task SetUserTimeZone_CannotBeSetTwiceWithIn24Hours()
        {
            _clockProvider.Now = new DateTime(2020, 4, 1, 0, 0, 0);

            await _settingAppService.SetUserTimeZone(new UserTimeZoneDto
            {
                // GMT +01:00
                TimeZone = "Central Europe Standard Time",
            });

            // UTC: 23:00 April 1; Local: 00:00 April 2
            _clockProvider.Now = _clockProvider.Now.AddHours(23);

            // should not set time zone
            await _settingAppService.SetUserTimeZone(new UserTimeZoneDto
            {
                TimeZone = "Dateline Standard Time",
            }).ShouldThrowAsync<UserFriendlyException>();

            // UTC: 00:00 April 2; Local: 01:00 April 2
            _clockProvider.Now = _clockProvider.Now.AddHours(1);

            // Can set time zone
            await _settingAppService.SetUserTimeZone(new UserTimeZoneDto
            {
                TimeZone = "Dateline Standard Time",
            });

            await UsingDbContextAsync(async ctx =>
            {
                var timeZoneOfUser =
                    await _userTimeZoneManager.GetTimeZoneOfUserAsync(GetHostAdmin().ToUserIdentifier());

                timeZoneOfUser.Id.ShouldBe("Dateline Standard Time");
            });
        }
    }
}
