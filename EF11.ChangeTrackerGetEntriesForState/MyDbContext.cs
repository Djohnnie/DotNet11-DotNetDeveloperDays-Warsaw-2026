using EF11.ChangeTrackerGetEntriesForState.Entities;
using Microsoft.EntityFrameworkCore;

namespace EF11.ChangeTrackerGetEntriesForState;

internal class MyDbContext : DbContext
{
    public DbSet<Blog> Blogs { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var connectionString = "Server=.\\SQLDEV;Database=EF11ChangeTrackerGetEntriesForState;Integrated Security=true;Trusted_Connection=True;Encrypt=false";
        optionsBuilder.UseSqlServer(connectionString);
    }
}
