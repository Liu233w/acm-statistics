using Microsoft.EntityFrameworkCore;
using Abp.Zero.EntityFrameworkCore;
using AcmStatisticsBackend.Authorization.Roles;
using AcmStatisticsBackend.Authorization.Users;
using AcmStatisticsBackend.MultiTenancy;

namespace AcmStatisticsBackend.EntityFrameworkCore
{
    public class AcmStatisticsBackendDbContext : AbpZeroDbContext<Tenant, Role, User, AcmStatisticsBackendDbContext>
    {
        /* Define a DbSet for each entity of the application */
        
        public AcmStatisticsBackendDbContext(DbContextOptions<AcmStatisticsBackendDbContext> options)
            : base(options)
        {
        }
    }
}
