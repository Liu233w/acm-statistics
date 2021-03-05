using System.Data.Common;
using Microsoft.EntityFrameworkCore;

namespace AcmStatisticsBackend.EntityFrameworkCore
{
    public static class AcmStatisticsBackendDbContextConfigurer
    {
        public static void Configure(DbContextOptionsBuilder<AcmStatisticsBackendDbContext> builder, string connectionString)
        {
            builder.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
        }

        public static void Configure(DbContextOptionsBuilder<AcmStatisticsBackendDbContext> builder, DbConnection connection)
        {
            builder.UseMySql(connection, ServerVersion.AutoDetect(connection.ConnectionString));
        }
    }
}
