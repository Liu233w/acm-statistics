using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNet.OData;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OHunt.Web.Database;
using OHunt.Web.Errors;
using OHunt.Web.Models;

namespace OHunt.Web.Controllers
{
    [Route("api/submissions")]
    [ApiController]
    public class SubmissionsController : ControllerBase
    {
        private readonly OHuntWebContext _context;
        private readonly ILogger<SubmissionsController> _logger;

        private static readonly List<string> SupportedOj
            = new List<string> { "zoj" };

        public SubmissionsController(
            OHuntWebContext context,
            ILogger<SubmissionsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: api/submissions/list
        [HttpGet]
        [Route("list")]
        public ICollection<string> GetSupportOj()
        {
            return SupportedOj;
        }

        // Get: api/submissions/status-names
        [HttpGet]
        [Route("status-names")]
        public string[] GetStatus()
        {
            return Enum.GetNames(typeof(RunResult));
        }

        // GET: api/submissions/oj/{zoj}
        [HttpGet]
        [EnableQuery(PageSize = 500)]
        [Route("oj/{oj}")]
        public IQueryable<Submission> GetSubmissions(string oj)
        {
            if (!Enum.TryParse<OnlineJudge>(oj, true, out var ojEnum))
            {
                throw new HttpResponseException
                {
                    Status = 400,
                    Value = new
                    {
                        Message = "Unrecognisable OJ name",
                    },
                };
            }

            return _context.Submission
                .Where(e => e.OnlineJudgeId == ojEnum);
        }
    }
}
