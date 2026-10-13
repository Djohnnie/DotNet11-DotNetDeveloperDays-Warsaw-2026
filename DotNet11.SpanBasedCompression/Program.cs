using System.Buffers;
using System.IO.Compression;
using System.Text;

// System.IO.Compression now offers Span<byte>/ReadOnlySpan<byte> encode and decode entry points
// for the Deflate, ZLib and GZip formats, mirroring the shape BrotliEncoder/BrotliDecoder already had.
// This avoids allocating a MemoryStream (and the DeflateStream/GZipStream/ZLibStream wrapper around it)
// when the data you're compressing already lives in a span - useful for protocol parsing, middleware,
// and other high-throughput scenarios that work directly with buffers.

byte[] source = Encoding.UTF8.GetBytes("Hello, .NET 11 span-based compression!");
Console.WriteLine($"Source: {source.Length} bytes");

Span<byte> compressed = new byte[source.Length + 64];
using (ZLibEncoder encoder = new())
{
    var status = encoder.Compress(source, compressed, out int consumed, out int written, isFinalBlock: true);
    Console.WriteLine($"Compress: {status}, consumed {consumed} bytes, wrote {written} compressed bytes");
    compressed = compressed[..written];
}

Span<byte> decompressed = new byte[source.Length];
using (ZLibDecoder decoder = new())
{
    var status = decoder.Decompress(compressed, decompressed, out int consumed, out int written);
    Console.WriteLine($"Decompress: {status}, consumed {consumed} compressed bytes, wrote {written} bytes");
    decompressed = decompressed[..written];
}

Console.WriteLine($"Round-tripped: {Encoding.UTF8.GetString(decompressed)}");
