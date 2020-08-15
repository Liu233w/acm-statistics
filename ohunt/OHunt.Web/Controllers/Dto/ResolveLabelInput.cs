using System.Collections.Generic;
using OHunt.Web.Models;

namespace OHunt.Web.Controllers.Dto
{
    public class ResolveLabelInput
    {
        /// <summary>
        /// The list to request. Each item is a problem id.
        /// </summary>
        public ICollection<int> List { get; set; }

        /// <summary>
        /// The oj of the problem list
        /// </summary>
        public MappingOnlineJudge OnlineJudge { get; set; }
    }
}
