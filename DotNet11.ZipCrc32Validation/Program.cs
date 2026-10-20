// .NET 11 Libraries - Validate ZIP checksums when reading.
// Introduced: Preview 3.
//
// ZIP entry reads now validate the stored CRC32. Corrupted or
// tampered entry data fails with
// InvalidDataException when the entry stream is read to the
// end, instead of silently handing
// back wrong bytes. (Preview 3 also moved the Zstandard APIs
// into System.IO.Compression.)
//
// Source:
// https://github.com/dotnet/core/blob/main/release-notes/11.0/preview/preview3/libraries.md#zstandard-moved-to-systemiocompression-and-zip-reads-validate-crc32

using System.IO.Compression;
using System.Text;

string zipPath = Path.Combine(Path.GetTempPath(), $"crc32-demo-{Guid.NewGuid():N}.zip");

try
{
    byte[] original = Encoding.UTF8.GetBytes("The quick brown fox jumps over the lazy dog.");

    // Store (not deflate) so the payload bytes are easy to
    // corrupt in place.
    using (var archive = ZipFile.Open(zipPath, ZipArchiveMode.Create))
    {
        ZipArchiveEntry entry = archive.CreateEntry("data.bin", CompressionLevel.NoCompression);
        using Stream entryStream = entry.Open();
        entryStream.Write(original);
    }

    Console.WriteLine("== Intact archive ==");
    Console.WriteLine(ReadEntry(zipPath, "data.bin"));

    CorruptStoredPayload(zipPath, original);

    Console.WriteLine();
    Console.WriteLine("== Corrupted archive ==");
    Console.WriteLine(ReadEntry(zipPath, "data.bin"));
}
finally
{
    if (File.Exists(zipPath))
    {
        File.Delete(zipPath);
    }
}

static string ReadEntry(string path, string entryName)
{
    try
    {
        using var archive = ZipFile.OpenRead(path);
        using Stream entry = archive.GetEntry(entryName)!.Open();
        using var destination = new MemoryStream();
        entry.CopyTo(destination);
        return $"read {destination.Length} byte(s): {Encoding.UTF8.GetString(destination.ToArray())}";
    }
    catch (InvalidDataException ex)
    {
        return $"InvalidDataException -> {ex.Message}";
    }
}

// Flip one byte of the stored (uncompressed) payload so the
// CRC32 no longer matches.
static void CorruptStoredPayload(string path, byte[] payload)
{
    byte[] bytes = File.ReadAllBytes(path);
    int offset = IndexOf(bytes, payload);
    bytes[offset] ^= 0xFF;
    File.WriteAllBytes(path, bytes);
}

static int IndexOf(byte[] haystack, byte[] needle)
{
    for (int i = 0; i + needle.Length <= haystack.Length; i++)
    {
        if (haystack.AsSpan(i, needle.Length).SequenceEqual(needle))
        {
            return i;
        }
    }

    throw new InvalidOperationException("Payload not found in the archive.");
}
