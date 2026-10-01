using System.IO.Compression;
using System.Text;

// .NET 11 adds first-class support for the Zstandard ("zstd") compression algorithm to
// System.IO.Compression, alongside the existing GZip/Deflate/Brotli streams. Zstandard offers
// a much better speed/ratio trade-off than GZip, which is why it's become the default choice
// for a lot of modern tooling (npm, container images, log shipping, ...).
// The new ZstandardStream follows the exact same Stream-based shape as GZipStream/BrotliStream,
// so it's a drop-in choice wherever you already compress data in .NET.

var text = string.Concat(Enumerable.Repeat("The quick brown fox jumps over the lazy dog. ", 200));
var original = Encoding.UTF8.GetBytes(text);

using var compressedStream = new MemoryStream();
using (var zstd = new ZstandardStream(compressedStream, CompressionLevel.Optimal, leaveOpen: true))
{
    zstd.Write(original);
}

Console.WriteLine($"Original size:   {original.Length,6} bytes");
Console.WriteLine($"Compressed size: {compressedStream.Length,6} bytes");
Console.WriteLine($"Ratio:           {(double)compressedStream.Length / original.Length:P1}");

compressedStream.Position = 0;
using var decompressedStream = new MemoryStream();
using (var zstd = new ZstandardStream(compressedStream, CompressionMode.Decompress))
{
    zstd.CopyTo(decompressedStream);
}

var roundTripped = decompressedStream.ToArray();
Console.WriteLine($"Round-trip matches original: {roundTripped.AsSpan().SequenceEqual(original)}");
