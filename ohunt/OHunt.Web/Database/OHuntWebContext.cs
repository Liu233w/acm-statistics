using Microsoft.EntityFrameworkCore;
using OHunt.Web.Models;

namespace OHunt.Web.Database
{
    public class OHuntWebContext : DbContext
    {
        public OHuntWebContext(DbContextOptions<OHuntWebContext> options)
            : base(options)
        {
        }

        public DbSet<Submission> Submission { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Submission>()
                .HasKey(submission => new { submission.SubmissionId, submission.OnlineJudgeId });
        }
    }
}
