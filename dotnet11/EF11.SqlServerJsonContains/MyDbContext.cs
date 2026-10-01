using EF11.SqlServerJsonContains.Entities;
using Microsoft.EntityFrameworkCore;

namespace EF11.SqlServerJsonContains;

internal class MyDbContext : DbContext
{
    public DbSet<Blog> Blogs { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var connectionString = "Server=.\\SQLDEV;Database=EF11SqlServerJsonContains;Integrated Security=true;Trusted_Connection=True;Encrypt=false";

        // JSON_CONTAINS() is a SQL Server 2025 function. EF Core only uses it once you tell EF
        // to target SQL Server 2025's compatibility level (170) - otherwise it falls back to the
        // older, less efficient OPENJSON-based translation so it keeps working on older servers.
        optionsBuilder.UseSqlServer(connectionString, o => o.UseCompatibilityLevel(170));
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Blog>().ComplexProperty(b => b.Details, b => b.ToJson());
    }
}
