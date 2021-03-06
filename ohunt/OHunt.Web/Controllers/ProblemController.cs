using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using OHunt.Web.Controllers.Dto;
using OHunt.Web.Services;

namespace OHunt.Web.Controllers
{
    [ApiController]
    [Route("/api/ohunt/problems/")]
    public class ProblemController : ControllerBase
    {
        private readonly ProblemLabelManager _labelManager;

        public ProblemController(ProblemLabelManager labelManager)
        {
            _labelManager = labelManager;
        }
        /// <summary>
        /// Resolve the actual label of a problem in OJ.
        /// </summary>
        [HttpPost]
        [Route("resolve-label")]
        [Produces("application/json")]
        public async Task<ResolveLabelOutput> ResolveLabel(ResolveLabelInput input)
        {
            var res = new Dictionary<string, string?>();

            foreach (var id in input.List)
            {
                var label = await _labelManager.ResolveProblemLabel(input.OnlineJudge, id);
                res.Add(id.ToString(), label);
            }

            return new ResolveLabelOutput
            {
                Result = res,
            };
        }
    }
}
