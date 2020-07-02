using System.Diagnostics;
using Flurl.Http;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNet.OData.Extensions;
using OHunt.Web.Crawlers;
using OHunt.Web.Database;
using OHunt.Web.Errors;
using OHunt.Web.Schedule;
using OHunt.Web.Utils;

namespace OHunt.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers(opt => { opt.Filters.Add(new HttpResponseExceptionFilter()); });

            services.AddDbContextPool<OHuntWebContext>(options =>
                options.UseMySql(Configuration.GetConnectionString("Default")));

            services.AddOData();

            services
                .AddSingleton<SubmissionCrawlerCoordinator>()
                .AddSingleton<SubmissionInserter>()
                .AddSingleton<ZojSubmissionCrawler>()
                ;

            // FlurlHttp.Configure(settings => settings.HttpClientFactory = new PollyHttpClientFactory());

            services.AddLogging();

            if (Configuration["DisableCrawlerWorker"] != "all")
            {
                services.AddHostedService<ScheduleCrawlerService>();
            }
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            UpdateDatabase(app);

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.EnableDependencyInjection();
                endpoints.Select().OrderBy().Filter().Count();
            });
        }

        private static void UpdateDatabase(IApplicationBuilder app)
        {
            using var serviceScope = app.ApplicationServices
                .GetRequiredService<IServiceScopeFactory>()
                .CreateScope();
            using var context = serviceScope.ServiceProvider
                .GetService<OHuntWebContext>();
            context.Database.Migrate();
        }
    }
}
