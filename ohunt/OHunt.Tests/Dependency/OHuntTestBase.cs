using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using OHunt.Web;
using OHunt.Web.Database;
using Xunit;
using Xunit.Abstractions;

namespace OHunt.Tests.Dependency
{
    public abstract class OHuntTestBase
        : IClassFixture<TestWebApplicationFactory<Startup>>
    {
        protected readonly WebApplicationFactory<Startup> Factory;

        protected OHuntTestBase(
            TestWebApplicationFactory<Startup> factory,
            ITestOutputHelper outputHelper)
        {
            Factory = factory.WithWebHostBuilder(ConfigureWebHost);
            factory.Output = outputHelper;
        }

        protected virtual void ConfigureWebHost(IWebHostBuilder builder)
        {
        }

        protected T WithDb<T>(Func<OHuntDbContext, T> func)
        {
            using var serviceScope = Factory.Services
                .CreateScope();
            using var context = serviceScope.ServiceProvider
                .GetService<OHuntDbContext>();
            return func(context);
        }

        protected void WithDb(Action<OHuntDbContext> func)
        {
            using var serviceScope = Factory.Services
                .CreateScope();
            using var context = serviceScope.ServiceProvider
                .GetService<OHuntDbContext>();
            func(context);
        }

        protected async Task<T> ResponseJson<T>(HttpResponseMessage message)
        {
            return JsonConvert.DeserializeObject<T>(await message.Content.ReadAsStringAsync());
        }
    }
}
