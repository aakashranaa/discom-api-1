// SolarApplicationDbContext.cs
using Microsoft.EntityFrameworkCore;
using Solar.Db.Tables;

public class DiscomDbContext : DbContext
{
        public DiscomDbContext(DbContextOptions<DiscomDbContext> options) : base(options) { }

        // DbSet properties and other configurations...

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Add any additional model configurations or overrides here
            // For example, you can specify indexes, relationships, etc.
        }

        public DbSet<SolarApplicationTable> SolarApplications { get; set; }
        public DbSet<State> tblState { get; set; } // Add this line for tblState

        public DbSet<Applicant> tblApplicant { get; set; }
        public DbSet<DiscomApplicationForm> tblDiscomApplicationForm { get; set; }
   
}
