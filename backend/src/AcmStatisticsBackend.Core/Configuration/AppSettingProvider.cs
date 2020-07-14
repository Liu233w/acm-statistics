using System.Collections.Generic;
using Abp.Configuration;

namespace AcmStatisticsBackend.Configuration
{
    public class AppSettingProvider : SettingProvider
    {
        public override IEnumerable<SettingDefinition> GetSettingDefinitions(SettingDefinitionProviderContext context)
        {
            return new[]
            {
                new SettingDefinition(AppSettingNames.UiTheme, "red",
                    scopes: SettingScopes.Application | SettingScopes.Tenant | SettingScopes.User,
                    isVisibleToClients: true),

                new SettingDefinition(AppSettingNames.AutoSaveHistory, "true", scopes: SettingScopes.User,
                    isVisibleToClients: true),
            };
        }
    }
}
