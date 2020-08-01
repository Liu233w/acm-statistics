using System;
using System.Linq;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OHunt.Web.Database;
using OHunt.Web.Services;
using Xunit.Abstractions;

namespace OHunt.Tests.Dependency
{
    public class TestWebApplicationFactory<TStartup>
        : WebApplicationFactory<TStartup> where TStartup : class
    {
        // Must be set in each test
        public ITestOutputHelper Output { get; set; } = null!;

        protected override IWebHostBuilder CreateWebHostBuilder()
        {
            var builder = base.CreateWebHostBuilder();
            builder.ConfigureLogging(logging =>
            {
                logging.ClearProviders(); // Remove other loggers
                logging.AddXUnit(Output); // Use the ITestOutputHelper instance
            });

            return builder;
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                // Remove the app's db context registration.
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType ==
                         typeof(DbContextOptions<OHuntDbContext>));

                if (descriptor != null)
                {
                    services.Remove(descriptor);
                }

                // Add db context using an in-memory database for testing.
                services.AddDbContext<OHuntDbContext>(
                    options =>
                    {
                        // use a new database each test
                        options.UseInMemoryDatabase(Guid.NewGuid().ToString());
                    }, ServiceLifetime.Scoped,
                    ServiceLifetime.Singleton);

                // remove ScheduleCrawlerService
                var service = services.SingleOrDefault(
                    d => d.ServiceType ==
                         typeof(ScheduleCrawlerService));

                if (service != null)
                {
                    services.Remove(service);
                }

                // replace database builder
                services.AddSingleton<IDbBuilder, NullDbBuilder>();

                // Build the service provider.
                var sp = services.BuildServiceProvider();
                SeedDatabase(sp);
            });
        }

        private static void SeedDatabase(IServiceProvider sp)
        {
            using var scope = sp.CreateScope();
            var scopedServices = scope.ServiceProvider;
            var db = scopedServices.GetRequiredService<OHuntDbContext>();
            var logger = scopedServices
                .GetRequiredService<ILogger<TestWebApplicationFactory<TStartup>>>();

            // Ensure the database is created.
            db.Database.EnsureCreated();

            try
            {
                // Seed the database with test data.
                // Utilities.InitializeDbForTests(db);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred seeding the " +
                                    "database with test messages. Error: {Message}", ex.Message);
            }
        }
    }
}
