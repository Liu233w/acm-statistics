using System.Data.Common;
using Microsoft.EntityFrameworkCore;

namespace AcmStatisticsBackend.EntityFrameworkCore
{
    public static class AcmStatisticsBackendDbContextConfigurer
    {
        public static void Configure(DbContextOptionsBuilder<AcmStatisticsBackendDbContext> builder,
            string connectionString)
        {
            builder
                .UseLazyLoadingProxies()
                .UseMySql(connectionString);
        }

        public static void Configure(DbContextOptionsBuilder<AcmStatisticsBackendDbContext> builder,
            DbConnection connection)
        {
            builder
                .UseLazyLoadingProxies()
                .UseMySql(connection);
        }
    }
}
