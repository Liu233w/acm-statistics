using System.Text;
using Microsoft.AspNet.OData.Builder;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNet.OData.Extensions;
using Microsoft.OData.Edm;
using OHunt.Web.Crawlers;
using OHunt.Web.Database;
using OHunt.Web.Errors;
using OHunt.Web.Models;
using OHunt.Web.Schedule;

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

            services.AddDbContextPool<OHuntDbContext>(options =>
                options.UseMySql(Configuration.GetConnectionString("Default")));

            services.AddOData();

            services.AddSingleton<IDbBuilder, OHuntDbBuilder>();

            services
                .AddSingleton<SubmissionCrawlerCoordinator>()
                .AddSingleton<DatabaseInserter<Submission>>()
                .AddSingleton<DatabaseInserter<CrawlerError>>()
                .AddSingleton<ZojSubmissionCrawler>()
                ;

            services.AddLogging();

            if (Configuration["DisableCrawlerWorker"] != "all")
            {
                services.AddHostedService<ScheduleCrawlerService>();
            }
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.ApplicationServices
                .GetService<IDbBuilder>()
                .BuildDatabase(app);

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                // endpoints.MapControllers();
                endpoints.EnableDependencyInjection();
                endpoints.Select().OrderBy().Filter().Count();
                endpoints.MapODataRoute("odata", "api/ohunt", GetEdmModel());
            });

            app.Use(async (ctx, next) =>
            {
                await next();
                if (ctx.Response.StatusCode == 404)
                {
                    await ctx.Response.Body.WriteAsync(
                        Encoding.ASCII.GetBytes("404 Not Found"));
                }
            });
        }

        private static IEdmModel GetEdmModel()
        {
            var odataBuilder = new ODataConventionModelBuilder();
            odataBuilder.EntitySet<Submission>("Submissions");

            return odataBuilder.GetEdmModel();
        }
    }
}
