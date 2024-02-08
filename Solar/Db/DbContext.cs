// SolarApplicationDbContext.cs
using Microsoft.Data.SqlClient;
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

        public DbSet<Area> tblArea { get; set; }
        public DbSet<DiscomApplicationForm> tblDiscomApplicationForm { get; set; }

   

    public async Task<AreaSection> GetAreaNameByAreaIdAsync(int areaId)
    {
        var areaIdParameter = new SqlParameter("@AreaId", areaId);
        var query = @"
        SELECT dbo.FnSDOBySectionID(@AreaId) AS AreaName
    ";

        var areaName = await Database.SqlQueryRaw<AreaSection>(query, areaIdParameter).FirstOrDefaultAsync();
        return areaName;
    }


}
