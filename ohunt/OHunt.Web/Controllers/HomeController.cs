using Microsoft.AspNetCore.Mvc;

namespace OHunt.Web.Controllers
{
    [Route("/")]
    public class HomeController : ControllerBase
    {
        [HttpGet("/")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public ActionResult Index()
        {
            return RedirectPermanent("/ohunt/swagger");
        }
    }
}
