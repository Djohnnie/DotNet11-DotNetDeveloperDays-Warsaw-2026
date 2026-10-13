using EF11.SqlServerVectorIndexes.Entities;
using Microsoft.EntityFrameworkCore;

namespace EF11.SqlServerVectorIndexes;

internal class MyDbContext : DbContext
{
    public DbSet<Blog> Blogs { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var connectionString = "Server=.\\SQLDEV;Database=EF11SqlServerVectorIndexes;Integrated Security=true;Trusted_Connection=True;Encrypt=false";
        optionsBuilder.UseSqlServer(connectionString);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Blog>(b =>
        {
            b.Property(x => x.Embedding).HasColumnType("vector(3)");

            // EF Core 11 can create SQL Server 2025 DiskANN vector indexes through migrations,
            // enabling *approximate* nearest-neighbor search - much faster than an exact scan
            // over EF.Functions.VectorDistance() once the table gets large.
            // NOTE: VECTOR_SEARCH() and vector indexes are still experimental in SQL Server 2025,
            // so this EF Core API is subject to change too.
            b.HasVectorIndex(x => x.Embedding)
                .HasMetric("cosine");
        });
    }
}
