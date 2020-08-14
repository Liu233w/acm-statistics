using System;
using System.Linq;
using Microsoft.AspNet.OData;
using Microsoft.AspNet.OData.Routing;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OHunt.Web.Database;
using OHunt.Web.Models;

namespace OHunt.Web.Controllers
{
    /// <summary>
    /// Get submissions
    /// </summary>
    [ODataRoutePrefix("submissions")]
    // for swagger
    [ApiExplorerSettings(IgnoreApi = false, GroupName = "Submissions")]
    [Route("api/ohunt/submissions")]
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

        /// <summary>
        /// Get the submission in an OJ. The api uses odata to create arbitrary query
        /// requests. See the document https://www.odata.org/ for more information.
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     GET /api/ohunt/submissions?oj=zoj&$filter=userName eq vjudge5 and status eq accepted&$count=true
        /// 
        /// </remarks>
        /// <param name="oj">the names of the OJ. See the OnlineJudge schema below for the value.</param>
        /// <returns>The query result. It is warped by odata.</returns>
        /// <response code="200">The correct result</response>
        /// <response code="301">If oj name is incorrect, redirect to a route and get correct oj names.</response>
        [EnableQuery(PageSize = 500)]
        [HttpGet("")]
        [Produces("application/json")]
        public ActionResult<IQueryable<Submission>> Get([FromODataUri] string oj)
        {
            if (!Enum.TryParse<OnlineJudge>(oj, true, out var ojEnum))
            {
                return RedirectToRoute("error");
            }

            return Ok(_context.Submission
                .Where(e => e.OnlineJudgeId == ojEnum));
        }

        [ApiExplorerSettings(IgnoreApi = true)]
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
