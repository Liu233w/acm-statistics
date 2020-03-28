using Abp.Web.Security.AntiForgery;
using AcmStatisticsBackend.Controllers;
using Microsoft.AspNetCore.Antiforgery;

namespace AcmStatisticsBackend.Web.Host.Controllers
{
    public class AntiForgeryController : AcmStatisticsBackendControllerBase
    {
        private readonly IAntiforgery _antiforgery;
        private readonly IAbpAntiForgeryManager _antiForgeryManager;

        public AntiForgeryController(IAntiforgery antiforgery, IAbpAntiForgeryManager antiForgeryManager)
        {
            _antiforgery = antiforgery;
            _antiForgeryManager = antiForgeryManager;
        }

        public void GetToken()
        {
            _antiforgery.SetCookieTokenAndHeader(HttpContext);
        }

        public void SetCookie()
        {
            _antiForgeryManager.SetCookie(HttpContext);
        }
    }
}
