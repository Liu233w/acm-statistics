using System.Threading.Tasks;
using AcmStatisticsBackend.Timing;
using Shouldly;
using Xunit;

namespace AcmStatisticsBackend.Tests.Timing
{
    public class UserTimeZoneManager_Tests : AcmStatisticsBackendTestBase
    {
        private readonly UserTimeZoneManager _userTimeZoneManager;

        public UserTimeZoneManager_Tests()
        {
            _userTimeZoneManager = Resolve<UserTimeZoneManager>();
        }

        [Fact]
        public async Task WhenNoTimeZoneSet_ShouldReturnDefaultTimeZone()
        {
            await UsingDbContextAsync(async ctx =>
            {
                var timeZone = await _userTimeZoneManager.GetTimeZoneOfUserAsync(
                    GetHostAdmin().ToUserIdentifier());

                timeZone.Id.ShouldBe("UTC");
            });
        }

        [Fact]
        public async Task WhenGettingTimeZone_ShouldReturnTimeZoneSet()
        {
            await _userTimeZoneManager.SetTimeZoneOfUserAsync(
                GetHostAdmin().ToUserIdentifier(),
                "Dateline Standard Time");

            var timeZone = await _userTimeZoneManager.GetTimeZoneOfUserAsync(
                GetHostAdmin().ToUserIdentifier());
            timeZone.Id.ShouldBe("Dateline Standard Time");
        }
    }
}
