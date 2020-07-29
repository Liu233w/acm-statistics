using System;
using System.Linq;
using Microsoft.AspNet.OData;
using Microsoft.AspNet.OData.Routing;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.OData;
using OHunt.Web.Database;
using OHunt.Web.Models;

namespace OHunt.Web.Controllers
{
    [ODataRoutePrefix("submissions")]
    public class SubmissionsController : ODataController
    {
        private readonly OHuntDbContext _context;
        private readonly ILogger<SubmissionsController> _logger;

        public SubmissionsController(
            OHuntDbContext context,
            ILogger<SubmissionsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: api/ohunt/submissions?oj=zoj&$filter=.....
        [EnableQuery(PageSize = 500)]
        public ActionResult<IQueryable<Submission>> Get([FromODataUri] string oj)
        {
            if (!Enum.TryParse<OnlineJudge>(oj, true, out var ojEnum))
            {
                return RedirectToRoute("error");
            }

            return Ok(_context.Submission
                .Where(e => e.OnlineJudgeId == ojEnum));
        }

        [HttpGet("error", Name = "error")]
        public ActionResult GetError()
        {
            return BadRequest(HelpObject);
        }

        private static readonly object HelpObject = new
        {
            Error = true,
            Message = "Unrecognisable OJ name",
            Detail = "Please use url like /api/ohunt/submissions?oj=zoj to request",
            SupportedOj = Enum.GetNames(typeof(OnlineJudge)),
        };
    }
}
