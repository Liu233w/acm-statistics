using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace OHunt.Web.Database
{
    public class OHuntDbBuilder : IDbBuilder
    {
        public void BuildDatabase(IApplicationBuilder app)
        {
            using var serviceScope = app.ApplicationServices
                .GetRequiredService<IServiceScopeFactory>()
                .CreateScope();
            using var context = serviceScope.ServiceProvider
                .GetService<OHuntDbContext>();
            context.Database.Migrate();
        }
    }
}
