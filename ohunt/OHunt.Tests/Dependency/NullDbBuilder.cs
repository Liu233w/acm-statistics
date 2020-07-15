using Microsoft.AspNetCore.Builder;
using OHunt.Web.Database;

namespace OHunt.Tests.Dependency
{
    public class NullDbBuilder : IDbBuilder
    {
        public void BuildDatabase(IApplicationBuilder app)
        {
            // pass
        }
    }
}
