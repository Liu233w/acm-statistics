using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OHunt.Web.Data;
using OHunt.Web.Models;

namespace OHunt.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SubmissionsController : ControllerBase
    {
        private readonly OHuntWebContext _context;

        public SubmissionsController(OHuntWebContext context)
        {
            _context = context;
        }

        // GET: api/Submissions
        [HttpGet]
        public IQueryable<Submission> GetSubmissions()
        {
            return _context.Submission.AsQueryable();
        }
    }
}
