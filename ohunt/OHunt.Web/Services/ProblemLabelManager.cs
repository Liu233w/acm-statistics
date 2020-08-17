using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using OHunt.Web.Crawlers;
using OHunt.Web.Database;
using OHunt.Web.Models;

namespace OHunt.Web.Services
{
    /// <summary>
    /// To manage the relation between problem label and id.
    /// </summary>
    /// <see cref="IMappingCrawler"/>
    public class ProblemLabelManager
    {
        private static readonly Type[] CrawlersToInit
            =
            {
                typeof(UvaMappingCrawler),
                typeof(UvaLiveMappingCrawler),
                typeof(NitMappingCrawler),
                typeof(BnuMappingCrawler),
            };

        private readonly Dictionary<MappingOnlineJudge, IMappingCrawler> _crawlers;
        private readonly OHuntDbContext _context;

        public ProblemLabelManager(OHuntDbContext context, IServiceProvider serviceProvider)
        {
            _context = context;

            _crawlers = new Dictionary<MappingOnlineJudge, IMappingCrawler>();
            foreach (var crawlerType in CrawlersToInit)
            {
                var crawler = (IMappingCrawler) serviceProvider.GetService(crawlerType);
                _crawlers.Add(crawler.OnlineJudge, crawler);
            }
        }

        /// <summary>
        /// Resolve the problem label from problem id. It can be read from database
        /// or queried by a crawler.
        /// </summary>
        /// <param name="oj">the oj to query</param>
        /// <param name="problemId">the id of the problem</param>
        /// <returns>the label. If it is null, the problem does not have a label.</returns>
        public async Task<string?> ResolveProblemLabel(
            MappingOnlineJudge oj,
            long problemId)
        {
            var mapping = await _context.ProblemLabelMappings.FirstOrDefaultAsync(
                e => e.ProblemId == problemId && e.OnlineJudgeId == oj);

            if (mapping != null)
            {
                return mapping.ProblemLabel;
            }

            if (!_crawlers.TryGetValue(oj, out var crawler))
            {
                throw new ProblemLabelManagerException($"The crawler of {oj.ToString()} does not exist");
            }

            var problemLabel = await crawler.GetProblemLabel(problemId);
            await _context.ProblemLabelMappings.AddAsync(new ProblemLabelMapping
            {
                ProblemId = problemId,
                OnlineJudgeId = oj,
                ProblemLabel = problemLabel,
            });
            await _context.SaveChangesAsync();

            return problemLabel;
        }
    }

    public class ProblemLabelManagerException : Exception
    {
        public ProblemLabelManagerException()
        {
        }

        protected ProblemLabelManagerException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public ProblemLabelManagerException(string? message) : base(message)
        {
        }

        public ProblemLabelManagerException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}
