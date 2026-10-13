using System.Text;
using System.Text.Json;

// JSON Lines (a.k.a. NDJSON) is one JSON value per line - a format widely used for logs, event
// streams and bulk-import files because it can be produced/consumed incrementally without ever
// holding the whole payload in memory. .NET 11 adds a new `topLevelValues` parameter to
// JsonSerializer.SerializeAsyncEnumerable: when true, each element is written as its own
// top-level JSON value separated by JsonSerializerOptions.NewLine, instead of being wrapped in
// a single JSON array.

var entries = new[]
{
    new LogEntry(new DateOnly(2026, 8, 25), "Info", "Server started"),
    new LogEntry(new DateOnly(2026, 8, 26), "Warning", "High memory usage"),
    new LogEntry(new DateOnly(2026, 8, 27), "Error", "Connection lost"),
};


using var stream = new MemoryStream();
await JsonSerializer.SerializeAsyncEnumerable(
    stream, AsAsyncEnumerable(entries), topLevelValues: true);


var jsonLines = Encoding.UTF8.GetString(stream.ToArray());
Console.WriteLine(jsonLines);




// Reading JSON Lines back is just: read line-by-line, deserialize each line on its own.
Console.WriteLine("--- Deserialized back ---");
using var reader = new StringReader(jsonLines);
string? line;
while ((line = reader.ReadLine()) is { Length: > 0 })
{
    var entry = JsonSerializer.Deserialize<LogEntry>(line);
    Console.WriteLine(entry);
}

static async IAsyncEnumerable<T> AsAsyncEnumerable<T>(IEnumerable<T> source)
{
    foreach (var item in source)
    {
        yield return item;
        await Task.Yield();
    }
}

record LogEntry(DateOnly Date, string Level, string Message);
