using Abp.AspNetCore.Mvc.Controllers;
using Abp.IdentityFramework;
using Microsoft.AspNetCore.Identity;

namespace AcmStatisticsBackend.Controllers
{
    public abstract class AcmStatisticsBackendControllerBase : AbpController
    {
        protected AcmStatisticsBackendControllerBase()
        {
            LocalizationSourceName = AcmStatisticsBackendConsts.LocalizationSourceName;
        }

        protected void CheckErrors(IdentityResult identityResult)
        {
            identityResult.CheckErrors(LocalizationManager);
        }
    }
}
