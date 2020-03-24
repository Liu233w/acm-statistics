using Abp.MultiTenancy;
using AcmStatisticsBackend.Authorization.Users;

namespace AcmStatisticsBackend.MultiTenancy
{
    public class Tenant : AbpTenant<User>
    {
        public Tenant()
        {
        }

        public Tenant(string tenancyName, string name)
            : base(tenancyName, name)
        {
        }
    }
}
