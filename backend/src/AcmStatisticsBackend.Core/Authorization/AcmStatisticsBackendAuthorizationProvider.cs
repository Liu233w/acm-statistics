using Abp.Authorization;
using Abp.Localization;
using Abp.MultiTenancy;

namespace AcmStatisticsBackend.Authorization
{
    public class AcmStatisticsBackendAuthorizationProvider : AuthorizationProvider
    {
        public override void SetPermissions(IPermissionDefinitionContext context)
        {
            context.CreatePermission(PermissionNames.Pages_Users, L("Users"));
            context.CreatePermission(PermissionNames.Pages_Roles, L("Roles"));
            context.CreatePermission(PermissionNames.Pages_Tenants, L("Tenants"),
                multiTenancySides: MultiTenancySides.Host);

            context.CreatePermission(PermissionNames.Statistics_DefaultQuery, F("Default query username"));
            context.CreatePermission(PermissionNames.AcHistory_Histories, F("Query history"));
            context.CreatePermission(PermissionNames.Settings_Update, F("Change user's own settings"));
        }

        private static ILocalizableString L(string name)
        {
            return new LocalizableString(name, AcmStatisticsBackendConsts.LocalizationSourceName);
        }

        private static ILocalizableString F(string content)
        {
            return new FixedLocalizableString(content);
        }
    }
}
