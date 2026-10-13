using EF11.CreateAndApplyMigrationsOneStep;
using EF11.CreateAndApplyMigrationsOneStep.Entities;
using Microsoft.EntityFrameworkCore;

// EF Core 11's "dotnet ef database update" command can now create AND apply a migration in a
// single step, using the new --add option. It uses Roslyn to compile the migration at runtime,
// which is handy for scenarios where recompiling the project isn't practical - e.g. .NET Aspire
// or containerized apps that only ship the already-built output.
//
//   dotnet ef database update InitialCreate --add
//
// This scaffolds a migration named "InitialCreate", compiles it with Roslyn, and immediately
// applies it to the database - all without a separate "dotnet ef migrations add" step first.
// The migration files are still written to disk under Migrations/ for source control, exactly
// as if you had run "migrations add" the normal way. The usual "migrations add" options work too:
//
//   dotnet ef database update AddProducts --add --output-dir Migrations/Products --namespace MyApp.Migrations
//
// In Visual Studio's Package Manager Console, the equivalent is the -Add switch:
//
//   Update-Database -Migration InitialCreate -Add
//
// This project's Migrations/ folder was generated with the classic two-step flow
// ("dotnet ef migrations add InitialMigration") since there's no live SQL Server instance in this
// environment to apply against - but the model and the generated migration are exactly what
// "database update InitialMigration --add" would have produced and applied in one go.

using var dbContext = new MyDbContext();

if (!await dbContext.Products.AnyAsync())
{
    dbContext.Products.AddRange(
        new Product { Name = "Keyboard", Price = 49.99m },
        new Product { Name = "Mouse", Price = 19.99m });

    await dbContext.SaveChangesAsync();
}

var products = await dbContext.Products.ToListAsync();
foreach (var product in products)
{
    Console.WriteLine($"{product.Name}: {product.Price:C}");
}
