using System.Data.Common;
using Microsoft.EntityFrameworkCore;

namespace AcmStatisticsBackend.EntityFrameworkCore
{
    public static class AcmStatisticsBackendDbContextConfigurer
    {
        public static void Configure(DbContextOptionsBuilder<AcmStatisticsBackendDbContext> builder, string connectionString)
        {
            builder.UseMySql(connectionString, new MySqlServerVersion(new System.Version(8, 0)));
        }

        public static void Configure(DbContextOptionsBuilder<AcmStatisticsBackendDbContext> builder, DbConnection connection)
        {
            builder.UseMySql(connection, new MySqlServerVersion(new System.Version(8, 0)));
        }
    }
}
