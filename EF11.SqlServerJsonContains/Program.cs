using EF11.SqlServerJsonContains;
using Microsoft.EntityFrameworkCore;

// SQL Server 2025 adds the JSON_CONTAINS() function, which checks whether a value exists in a
// JSON document. Starting with EF Core 11, when targeting SQL Server 2025 (compatibility level
// 170, see MyDbContext.OnConfiguring), LINQ Contains() queries over primitive collections stored
// as JSON are translated to JSON_CONTAINS() instead of the older, less efficient OPENJSON-based
// translation - and JSON_CONTAINS() can make use of a JSON index, if one is defined.
//
// EF Core 11 also adds EF.Functions.JsonContains() for cases where you want to call
// JSON_CONTAINS() directly yourself, e.g. to search at a specific JSON path.

using var dbContext = new MyDbContext();

// Automatically translated to JSON_CONTAINS([b].[Tags], 'ef-core') on SQL Server 2025.
var byTag = dbContext.Blogs.Where(b => b.Tags.Contains("ef-core"));
Console.WriteLine("Contains() over a JSON-mapped List<string> (auto-translated to JSON_CONTAINS):");
Console.WriteLine(byTag.ToQueryString());

// Explicit EF.Functions.JsonContains() call, searching at a specific JSON path.
var byRating = dbContext.Blogs.Where(b => EF.Functions.JsonContains(b.Details, 8, "$.Rating") == 1);
Console.WriteLine();
Console.WriteLine("EF.Functions.JsonContains() at an explicit JSON path:");
Console.WriteLine(byRating.ToQueryString());
