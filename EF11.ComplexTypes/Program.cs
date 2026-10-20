using EF11.ComplexTypes;
using EF11.ComplexTypes.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

// EF Core 11 - Use complex types with more mappings.
// Introduced: Preview 1 (complex types with TPT/TPC and JSON);
// Preview 6 (keys and indexes
// through complex properties); Preview 7 (additional provider
// support and refinements).
//
// Complex types are value objects without their own identity.
// EF Core 11 lets them work with
// TPT/TPC inheritance, JSON columns and Cosmos DB, and - new in
// Preview 6 - lets you declare
// scalar keys and indexes through a complex property using a
// member-access path such as
// HasIndex(c => c.Address.PostalCode). See
// MyDbContext.OnModelCreating for the configuration.
//
// Source:
// https://learn.microsoft.com/en-us/ef/core/what-is-new/ef-core-11.0/whatsnew#complex-types
//
// This demo only inspects the model and the generated SQL, so
// it never opens a connection.

using var dbContext = new MyDbContext();

IEntityType customer = dbContext.Model.FindEntityType(typeof(Customer))!;

Console.WriteLine("== Complex properties on Customer ==");
foreach (IComplexProperty complexProperty in customer.GetComplexProperties())
{
    bool json = complexProperty.ComplexType.GetContainerColumnName() is not null;
    Console.WriteLine($"  {complexProperty.Name} ({(json ? "JSON column" : "inline columns")})");
    PrintComplexType(complexProperty.ComplexType, indent: 4);
}

Console.WriteLine();
Console.WriteLine("== Indexes declared through complex properties ==");
foreach (IIndex index in customer.GetIndexes())
{
    string columns = string.Join(", ", index.Properties.Select(p => p is IProperty prop ? prop.GetColumnName() : p.Name));
    Console.WriteLine($"  INDEX ON ({columns})");
}

Console.WriteLine();
Console.WriteLine("== Querying through a complex property ==");
IQueryable<Customer> query = dbContext.Customers
    .Where(c => c.ShippingAddress.Country.IsoCode == "PL" && c.ShippingAddress.City == "Warsaw")
    .OrderBy(c => c.ShippingAddress.PostalCode);

Console.WriteLine(query.ToQueryString());

static void PrintComplexType(IComplexType complexType, int indent)
{
    string pad = new(' ', indent);

    foreach (IProperty property in complexType.GetProperties())
    {
        Console.WriteLine($"{pad}{property.Name} -> {property.GetColumnName() ?? "(json)"}");
    }

    foreach (IComplexProperty nested in complexType.GetComplexProperties())
    {
        Console.WriteLine($"{pad}{nested.Name} (nested complex type)");
        PrintComplexType(nested.ComplexType, indent + 2);
    }
}
