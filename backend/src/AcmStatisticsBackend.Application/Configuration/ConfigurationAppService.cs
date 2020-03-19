using System.Threading.Tasks;
using Abp.Authorization;
using Abp.Runtime.Session;
using AcmStatisticsBackend.Configuration.Dto;

namespace AcmStatisticsBackend.Configuration
{
    [AbpAuthorize]
    public class ConfigurationAppService : AcmStatisticsBackendAppServiceBase, IConfigurationAppService
    {
        public async Task ChangeUiTheme(ChangeUiThemeInput input)
        {
            await SettingManager.ChangeSettingForUserAsync(AbpSession.ToUserIdentifier(), AppSettingNames.UiTheme, input.Theme);
        }
    }
}
