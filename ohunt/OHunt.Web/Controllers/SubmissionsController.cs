using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNet.OData;
using Microsoft.AspNetCore.Mvc;
using OHunt.Web.Data;
using OHunt.Web.Models;

namespace OHunt.Web.Controllers
{
    [Route("api/submissions")]
    [ApiController]
    public class SubmissionsController : ControllerBase
    {
        private readonly OHuntWebContext _context;

        private static readonly List<string> SUPPORTED_OJ
            = new List<string> { "zoj" };

        public SubmissionsController(OHuntWebContext context)
        {
            _context = context;
        }

        // GET: api/submissions/list
        [HttpGet]
        [Route("list")]
        public ICollection<string> GetSupportOj()
        {
            return SUPPORTED_OJ;
        }

        // GET: api/submissions/oj/{zoj}
        [HttpGet]
        [EnableQuery(PageSize=500)]
        [Route("oj/{oj}")]
        public IQueryable<Submission> GetSubmissions(string oj)
        {
            return _context.Submission
                .Where(e => e.OjName == oj);
        }
    }
}
