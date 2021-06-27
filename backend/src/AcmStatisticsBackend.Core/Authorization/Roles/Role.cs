using System.ComponentModel.DataAnnotations;
using Abp.Authorization.Roles;
using AcmStatisticsBackend.Authorization.Users;

namespace AcmStatisticsBackend.Authorization.Roles
{
    public class Role : AbpRole<User>
    {
        public Role()
        {
        }

        public Role(int? tenantId, string displayName)
            : base(tenantId, displayName)
        {
        }

        public Role(int? tenantId, string name, string displayName)
            : base(tenantId, name, displayName)
        {
        }
    }
}
