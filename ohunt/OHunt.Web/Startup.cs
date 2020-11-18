using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using Microsoft.AspNet.OData.Builder;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNet.OData.Extensions;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Net.Http.Headers;
using Microsoft.OData.Edm;
using Microsoft.OpenApi.Models;
using OHunt.Web.Crawlers;
using OHunt.Web.Database;
using OHunt.Web.Dataflow;
using OHunt.Web.Models;
using OHunt.Web.Options;
using OHunt.Web.Services;
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
            var conn = Configuration.GetConnectionString("Default");
            services.AddDbContextPool<OHuntDbContext>(options =>
                options.UseMySql(conn,
                ServerVersion.AutoDetect(conn)));

            services.AddOData();

            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo()
                {
                    Title = "OHunt API",
                    Version = "v1",
                    Description =
                        "OHunt is a crawler that reads data from online coding competition platform and serve them as API, just like uHunt of UVA.",
                });
                options.DocInclusionPredicate((docName, description) => true);

                // Set the comments path for the swagger json and ui.
                var basePath = AppDomain.CurrentDomain.BaseDirectory!;
                var docPath = Path.Combine(basePath, "OHunt.Web.xml");
                options.IncludeXmlComments(docPath);

                // use filter to add swagger document
                options.OperationFilter<QueryParameterFilter>();
            });

            services.AddControllers(options =>
                {
                    foreach (var outputFormatter in options.OutputFormatters.OfType<OutputFormatter>()
                        .Where(x => x.SupportedMediaTypes.Count == 0))
                    {
                        outputFormatter.SupportedMediaTypes.Add(
                            new MediaTypeHeaderValue("application/prs.odatatestxx-odata"));
                    }

                    foreach (var inputFormatter in options.InputFormatters.OfType<InputFormatter>()
                        .Where(x => x.SupportedMediaTypes.Count == 0))
                    {
                        inputFormatter.SupportedMediaTypes.Add(
                            new MediaTypeHeaderValue("application/prs.odatatestxx-odata"));
                    }
                })
                .AddJsonOptions(opts =>
                {
                    // use string as enum
                    opts.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                });

            services.AddSingleton<IDbBuilder, OHuntDbBuilder>();

            services
                .AddSingleton<SubmissionCrawlerCoordinator>()
                .AddSingleton<DatabaseInserterFactory>()
                .AddSingleton<ZojSubmissionCrawler>()
                .AddTransient<ProblemLabelManager>()
                .AddSingleton<UvaMappingCrawler>()
                .AddSingleton<UvaLiveMappingCrawler>()
                .AddSingleton<NitMappingCrawler>()
                .AddSingleton<BnuMappingCrawler>()
                ;

            services.AddLogging();

            if (Configuration["DisableCrawlerWorker"] != "all")
            {
                services.AddHostedService<ScheduleCrawlerService>();
            }

            services.Configure<DatabaseInserterOptions>(Configuration.GetSection("DatabaseInserter"));
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

            app.UseSwagger(opts =>
            {
                //
                opts.RouteTemplate = "api/ohunt/{documentName}/swagger.json";
            });
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/api/ohunt/v1/swagger.json", "OHunt API V1");
                c.RoutePrefix = "ohunt/swagger";
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
