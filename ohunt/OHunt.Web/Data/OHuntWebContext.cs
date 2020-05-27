using Microsoft.EntityFrameworkCore;
using OHunt.Web.Models;

namespace OHunt.Web.Data
{
    public class OHuntWebContext : DbContext
    {
        public OHuntWebContext (DbContextOptions<OHuntWebContext> options)
            : base(options)
        {
        }

        public DbSet<Submission> Submission { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Submission>()
                .HasIndex(e => e.OjName);
        }
    }
}
