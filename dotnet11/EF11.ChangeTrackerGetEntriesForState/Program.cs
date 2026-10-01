using EF11.ChangeTrackerGetEntriesForState;
using EF11.ChangeTrackerGetEntriesForState.Entities;
using Microsoft.EntityFrameworkCore;

// EF Core 11 adds ChangeTracker.GetEntriesForState(added, modified, deleted, unchanged).
// Previously, getting the entries for a specific EntityState meant calling ChangeTracker.Entries()
// (which runs a full DetectChanges() pass over every tracked entity) and then filtering with LINQ.
// GetEntriesForState() lets you ask directly for the states you care about, avoiding that extra
// change-detection pass - handy in long-lived contexts or high-volume change-tracking code.
//
// This demo only needs an in-memory change tracker, so it never touches the database.

using var dbContext = new MyDbContext();

var untouched = new Blog { Id = 1, Name = ".NET Blog" };
var toUpdate = new Blog { Id = 2, Name = "ASP.NET Blog" };
var toDelete = new Blog { Id = 3, Name = "Old Blog" };
var toAdd = new Blog { Id = 0, Name = "New Blog" };

dbContext.Attach(untouched);
dbContext.Attach(toUpdate);
dbContext.Attach(toDelete);

toUpdate.Name = "ASP.NET Core Blog";
dbContext.Remove(toDelete);
dbContext.Add(toAdd);

// Old way: ChangeTracker.Entries() runs DetectChanges() and then you filter with LINQ.
var modifiedTheOldWay = dbContext.ChangeTracker.Entries<Blog>()
    .Where(e => e.State == EntityState.Modified)
    .ToList();

// New way: ask directly for the states you care about - no extra DetectChanges() pass needed
// when you already know which states matter.
var addedOrModified = dbContext.ChangeTracker.GetEntriesForState(
    added: true,
    modified: true,
    deleted: false,
    unchanged: false);

Console.WriteLine($"Old way found {modifiedTheOldWay.Count} modified entries.");
Console.WriteLine("New way (GetEntriesForState) found:");
foreach (var entry in addedOrModified)
{
    var blog = (Blog)entry.Entity;
    Console.WriteLine($" - {blog.Name} is {entry.State}");
}
