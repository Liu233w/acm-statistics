using Microsoft.AspNetCore.Builder;

namespace OHunt.Web.Database
{
    public interface IDbBuilder
    {
        void BuildDatabase(IApplicationBuilder app);
    }
}
