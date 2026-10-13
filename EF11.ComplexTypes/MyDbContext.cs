using EF11.ComplexTypes.Entities;
using Microsoft.EntityFrameworkCore;

namespace EF11.ComplexTypes;

internal class MyDbContext : DbContext
{
    public DbSet<Customer> Customers { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var connectionString = "Server=.\\SQLDEV;Database=EF11ComplexTypes;Integrated Security=true;Trusted_Connection=True;Encrypt=false";
        optionsBuilder.UseSqlServer(connectionString);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Customer>(customer =>
        {
            // Complex properties can nest. Country is a complex type inside Address,
            // which is itself a complex type on Customer.
            customer.ComplexProperty(c => c.ShippingAddress, address =>
            {
                address.IsRequired();
                address.ComplexProperty(a => a.Country).IsRequired();
            });

            // EF Core 11 lets a complex property be mapped to a JSON column.
            customer.ComplexProperty(c => c.BillingAddress, address =>
            {
                address.IsRequired();
                address.ToJson();
                address.ComplexProperty(a => a.Country).IsRequired();
            });

            // EF Core 11 Preview 6: keys and indexes can be declared through a
            // complex property using a member-access path.
            customer.HasIndex(c => c.ShippingAddress.PostalCode);
            customer.HasIndex(c => new { c.ShippingAddress.Country.IsoCode, c.ShippingAddress.City });
        });
    }
}
