using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNet.OData;
using Microsoft.AspNet.OData.Routing;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OHunt.Web.Database;
using OHunt.Web.Errors;
using OHunt.Web.Models;

namespace OHunt.Web.Controllers
{
    [ODataRoutePrefix("submissions")]
    public class SubmissionsController : ODataController
    {
        private readonly OHuntWebContext _context;
        private readonly ILogger<SubmissionsController> _logger;

        public SubmissionsController(
            OHuntWebContext context,
            ILogger<SubmissionsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: api/ohunt/submissions?oj=zoj&$filter=.....
        [EnableQuery(PageSize = 500)]
        public IQueryable<Submission> Get([FromODataUri] string oj)
        {
            if (!Enum.TryParse<OnlineJudge>(oj, true, out var ojEnum))
            {
                throw new HttpResponseException
                {
                    Status = 400,
                    Value = new
                    {
                        Error = true,
                        Message = "Unrecognisable OJ name",
                        Detail = "Please use url like /api/ohunt/submissions?oj=zoj to request",
                        SupportedOj = Enum.GetNames(typeof(OnlineJudge)),
                    },
                };
            }

            return _context.Submission
                .Where(e => e.OnlineJudgeId == ojEnum);
        }
    }
}
