using AcmStatisticsBackend.Configuration;
using AcmStatisticsBackend.Web;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace AcmStatisticsBackend.EntityFrameworkCore
{
    /* This class is needed to run "dotnet ef ..." commands from command line on development. Not used anywhere else */
    public class AcmStatisticsBackendDbContextFactory : IDesignTimeDbContextFactory<AcmStatisticsBackendDbContext>
    {
        public AcmStatisticsBackendDbContext CreateDbContext(string[] args)
        {
            var builder = new DbContextOptionsBuilder<AcmStatisticsBackendDbContext>();
            var configuration = AppConfigurations.Get(WebContentDirectoryFinder.CalculateContentRootFolder());

            AcmStatisticsBackendDbContextConfigurer.Configure(builder, configuration.GetConnectionString(AcmStatisticsBackendConsts.ConnectionStringName));

            return new AcmStatisticsBackendDbContext(builder.Options);
        }
    }
}
