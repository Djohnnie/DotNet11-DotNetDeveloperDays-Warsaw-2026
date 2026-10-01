using EF11.SqlServerVectorIndexes;
using EF11.SqlServerVectorIndexes.Entities;
using Microsoft.Data.SqlTypes;
using Microsoft.EntityFrameworkCore;

// EF Core 10 introduced EF.Functions.VectorDistance(), which computes an *exact* distance between
// two vectors - great for accuracy, but it has to scan every row.
//
// SQL Server 2025 also supports *approximate* nearest-neighbor search over a vector index, which
// trades a little accuracy for a lot of speed on large datasets. EF Core 11 can create these
// DiskANN vector indexes through migrations (see MyDbContext.OnModelCreating -> HasVectorIndex),
// and exposes a VectorSearch() extension method - chained with Take() and WithApproximate() - to
// query through that index. Without WithApproximate() it falls back to an exact k-NN search.

using var dbContext = new MyDbContext();

var queryEmbedding = new SqlVector<float>(new float[] { 0.1f, 0.2f, 0.3f });

// Approximate nearest-neighbor search using the DiskANN vector index created above.
// Generates SQL that calls SQL Server's VECTOR_SEARCH() table-valued function
// with "WITH APPROXIMATE" on the TOP clause.
var approximateQuery = dbContext.Blogs
    .VectorSearch(b => b.Embedding, queryEmbedding, "cosine")
    .OrderBy(r => r.Distance)
    .Take(5)
    .WithApproximate();

Console.WriteLine("Approximate vector search query (VECTOR_SEARCH + vector index):");
Console.WriteLine(approximateQuery.ToQueryString());

// For comparison: an exact search using VectorDistance(), which scans every row instead of
// using the index.
var exactQuery = dbContext.Blogs
    .OrderBy(b => EF.Functions.VectorDistance("cosine", b.Embedding, queryEmbedding))
    .Take(5);

Console.WriteLine();
Console.WriteLine("Exact vector search query (VectorDistance, no index used):");
Console.WriteLine(exactQuery.ToQueryString());
