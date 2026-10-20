using EF11.LinqTranslations;
using EF11.LinqTranslations.Entities;
using Microsoft.EntityFrameworkCore;

// EF Core 11 - Translate more LINQ and generate leaner SQL.
// Introduced: Preview 2 (MaxBy/MinBy); Preview 3 (join
// pruning); Preview 5 (cleaner SQL);
// Preview 6 (FullJoin); Preview 7 (GroupBy enhancements).
//
// EF Core 11 adds relational FullJoin translation, expands
// GroupBy, and translates
// MaxBy/MinBy. The query pipeline also prunes redundant joins,
// ordering keys and no-op casts,
// so the generated SQL is noticeably leaner.
//
// Source:
// https://learn.microsoft.com/en-us/ef/core/what-is-new/ef-core-11.0/whatsnew#linq-and-sql-translation
//
// This demo only prints the translated SQL via ToQueryString(),
// so it never opens a connection.

using var dbContext = new MyDbContext();

Show("FullJoin - both sides are kept, new in Preview 6",
    dbContext.Customers
        .FullJoin(
            dbContext.Orders,
            customer => customer.Id,
            order => order.CustomerId,
            // FullJoin keeps unmatched rows on both sides, so
            // either element can be null.
            (customer, order) => new
            {
                CustomerName = customer != null ? customer.Name : null,
                OrderTotal = order != null ? (decimal?)order.Total : null,
            })
        .ToQueryString());

Show("MaxBy / MinBy now translate to SQL",
    dbContext.Customers
        .Select(c => new
        {
            c.Name,
            BiggestOrder = c.Orders.MaxBy(o => o.Total)!.Total,
            FirstOrder = c.Orders.MinBy(o => o.PlacedOn)!.PlacedOn,
        })
        .ToQueryString());

Show("Expanded GroupBy support",
    dbContext.Orders
        .GroupBy(o => new { o.Customer.Country, Year = o.PlacedOn.Year })
        .Select(g => new
        {
            g.Key.Country,
            g.Key.Year,
            Orders = g.Count(),
            Revenue = g.Sum(o => o.Total),
            Largest = g.Max(o => o.Total),
        })
        .OrderByDescending(x => x.Revenue)
        .ToQueryString());

Show("Join pruning - the Customer join is not needed and disappears",
    dbContext.Orders
        .Where(o => o.Total > 100m)
        .Select(o => new { o.Id, o.CustomerId })
        .ToQueryString());

Show("No-op casts are removed",
    dbContext.Orders
        .Where(o => (decimal)o.Total > 10m)
        .Select(o => (int)o.Id)
        .ToQueryString());

static void Show(string title, string sql)
{
    Console.WriteLine($"== {title} ==");
    Console.WriteLine(sql);
    Console.WriteLine();
}
