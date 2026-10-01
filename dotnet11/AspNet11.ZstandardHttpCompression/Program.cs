using Microsoft.AspNetCore.ResponseCompression;
using System.IO.Compression;

// ASP.NET Core now supports Zstandard (zstd) for both response compression and request
// decompression. This adds zstd support to the existing response-compression and
// request-decompression middleware, alongside the existing Brotli/Gzip providers, and the
// response-compression middleware picks it automatically whenever a client's Accept-Encoding
// header prefers it.
//
// Try it (from a separate terminal, while this app is running):
//   curl -s -H "Accept-Encoding: zstd" -o out.zst -D - http://localhost:5000/lorem
//   # -> "Content-Encoding: zstd" in the response headers, and out.zst is a valid zstd stream.

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddResponseCompression();
builder.Services.AddRequestDecompression();
builder.Services.Configure<ZstandardCompressionProviderOptions>(options =>
{
    options.CompressionOptions = new ZstandardCompressionOptions
    {
        Quality = 6 // 1-22, higher = better compression, slower
    };
});

var app = builder.Build();

app.UseResponseCompression();
app.UseRequestDecompression();

// A reasonably large, highly-compressible payload so the effect of compression is visible.
app.MapGet("/lorem", () => string.Concat(Enumerable.Repeat(
    "The quick brown fox jumps over the lazy dog. .NET 11 adds Zstandard compression. ", 200)));

// Accepts a request body that the client may have zstd-compressed on the way in - the request
// decompression middleware transparently decompresses it before the endpoint sees it.
app.MapPost("/echo", async (HttpRequest request) =>
{
    using var reader = new StreamReader(request.Body);
    return await reader.ReadToEndAsync();
});

app.Run();
