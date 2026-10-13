// Requires the .NET 11 Preview 7 SDK or later - this API isn't present yet in Preview 6.
//
// .NET 11 adds native password protection to System.IO.Compression's ZipArchive:
// - ZipArchive.CreateEntry(name, password, encryptionMethod) creates an encrypted entry.
// - ZipArchiveEntry.Open(password) / OpenAsync(password) reads it back.
// - ZipEncryptionMethod exposes ZipCrypto (legacy) and WinZip AES-128/192/256.
// A wrong password throws InvalidDataException when the entry is opened.

using System.IO.Compression;

var zipPath = Path.Combine(Path.GetTempPath(), $"dotnet11-secrets-{Guid.NewGuid():N}.zip");
const string correctPassword = "correct horse battery staple";
const string wrongPassword = "hunter2";

try
{
    using (var archive = ZipFile.Open(zipPath, ZipArchiveMode.Create))
    {
        ZipArchiveEntry entry = archive.CreateEntry(
            "notes.txt",
            password: correctPassword,
            encryptionMethod: ZipEncryptionMethod.Aes256);

        using Stream entryStream = entry.Open(correctPassword);
        using var writer = new StreamWriter(entryStream);
        writer.WriteLine("Encrypted contents.");
    }

    Console.WriteLine($"Created AES-256 encrypted zip: {zipPath}");

    using (var archive = ZipFile.OpenRead(zipPath))
    {
        var entry = archive.GetEntry("notes.txt")!;
        Console.WriteLine($"Entry encryption method: {entry.EncryptionMethod}");

        using var stream = entry.Open(correctPassword);
        using var reader = new StreamReader(stream);
        Console.WriteLine($"Read back with the correct password: {reader.ReadLine()}");
    }

    using (var archive = ZipFile.OpenRead(zipPath))
    {
        var entry = archive.GetEntry("notes.txt")!;
        try
        {
            using var stream = entry.Open(wrongPassword);
            using var reader = new StreamReader(stream);
            reader.ReadToEnd();
            Console.WriteLine("Unexpectedly succeeded with the wrong password!");
        }
        catch (InvalidDataException)
        {
            Console.WriteLine("Wrong password correctly rejected with InvalidDataException.");
        }
    }
}
finally
{
    File.Delete(zipPath);
}
