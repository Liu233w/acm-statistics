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

            context.CreatePermission(PermissionNames.Statistics_DefaultQuery, new FixedLocalizableString("默认用户名"));
            context.CreatePermission(PermissionNames.AcHistory_Histories, new FixedLocalizableString("查询历史"));
        }

        private static ILocalizableString L(string name)
        {
            return new LocalizableString(name, AcmStatisticsBackendConsts.LocalizationSourceName);
        }
    }
}
