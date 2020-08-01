using Microsoft.EntityFrameworkCore;
using OHunt.Web.Models;

namespace OHunt.Web.Database
{
    public class OHuntDbContext : DbContext
    {
        public OHuntDbContext(DbContextOptions<OHuntDbContext> options)
            : base(options)
        {
        }

        public DbSet<Submission> Submission { get; set; } = null!;

        public DbSet<CrawlerError> CrawlerErrors { get; set; } = null!;

        public DbSet<ProblemLabelMapping> ProblemLabelMappings { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Submission>()
                .HasKey(submission => new { submission.SubmissionId, submission.OnlineJudgeId });

            modelBuilder.Entity<Submission>()
                .HasIndex(e => e.UserName);
            modelBuilder.Entity<Submission>()
                .HasIndex(e => e.Status);
        }
    }
}
