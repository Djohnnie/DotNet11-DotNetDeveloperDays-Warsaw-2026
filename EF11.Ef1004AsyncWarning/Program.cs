using EF11.Ef1004AsyncWarning;
using Microsoft.EntityFrameworkCore;

// EF Core 11 ships a new Roslyn analyzer diagnostic, EF1004, which fires when you call the
// generic System.Linq.AsyncEnumerable.ToAsyncEnumerable() extension method on an EF Core
// IQueryable<T>. That method exists for *any* IEnumerable/IQueryable - it just wraps synchronous
// enumeration in an async-looking API, so it still evaluates the EF query *synchronously* under
// the hood, defeating the purpose of "going async" and blocking a thread while the database
// responds. EF Core has its own AsAsyncEnumerable() that correctly uses the async query pipeline
// instead, and EF1004 nudges you towards it (with a code fix that swaps the call automatically).
//
// Build this project (or open it in an IDE) to see the actual EF1004 warning reported below.

using var dbContext = new MyDbContext();

var blogsQuery = dbContext.Blogs.Where(b => b.Name.StartsWith(".NET"));

// EF1004: ToAsyncEnumerable() on an IQueryable<T> evaluates the query synchronously - don't do this.
await foreach (var blog in blogsQuery.ToAsyncEnumerable())
{
    Console.WriteLine(blog.Name);
}

// Fixed: AsAsyncEnumerable() uses EF Core's real async query pipeline.
await foreach (var blog in blogsQuery.AsAsyncEnumerable())
{
    Console.WriteLine(blog.Name);
}
