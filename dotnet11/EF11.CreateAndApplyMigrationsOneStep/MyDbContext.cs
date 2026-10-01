using EF11.CreateAndApplyMigrationsOneStep.Entities;
using Microsoft.EntityFrameworkCore;

namespace EF11.CreateAndApplyMigrationsOneStep;

internal class MyDbContext : DbContext
{
    public DbSet<Product> Products { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var connectionString = "Server=.\\SQLDEV;Database=EF11CreateAndApplyMigrationsOneStep;Integrated Security=true;Trusted_Connection=True;Encrypt=false";
        optionsBuilder.UseSqlServer(connectionString);
    }
}
