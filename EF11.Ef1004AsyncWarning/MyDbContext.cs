using EF11.Ef1004AsyncWarning.Entities;
using Microsoft.EntityFrameworkCore;

namespace EF11.Ef1004AsyncWarning;

internal class MyDbContext : DbContext
{
    public DbSet<Blog> Blogs { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var connectionString = "Server=.\\SQLDEV;Database=EF11Ef1004AsyncWarning;Integrated Security=true;Trusted_Connection=True;Encrypt=false";
        optionsBuilder.UseSqlServer(connectionString);
    }
}
