// Requires the .NET 11 Preview 7 SDK or later - this API isn't present yet in Preview 6.
//
// .NET 11 adds culture-agnostic ToUpperOrdinal/ToLowerOrdinal to char, string, Rune and MemoryExtensions
// (so it also works on ReadOnlySpan<char> without allocating). Ordinal casing uses the same simple
// one-to-one mapping as OrdinalIgnoreCase comparisons, so it stays deterministic regardless of the
// current culture - unlike ToUpper()/ToLower(), which are culture-sensitive and can produce surprising
// results (the classic "Turkish I" problem: in tr-TR, "i".ToUpper() is "İ", not "I").

using System.Globalization;

string userInput = "istanbul";

CultureInfo.CurrentCulture = new CultureInfo("tr-TR");
string turkishUpper = userInput.ToUpper();
string ordinalUpper = userInput.ToUpperOrdinal();

Console.WriteLine($"Current culture:                   {CultureInfo.CurrentCulture.Name}");
Console.WriteLine($"Culture-sensitive ToUpper():        {turkishUpper}");
Console.WriteLine($"Culture-agnostic ToUpperOrdinal():  {ordinalUpper}");
Console.WriteLine($"Pairs with StringComparer.Ordinal:  {string.Equals(ordinalUpper, "ISTANBUL", StringComparison.Ordinal)}");

// Also available on spans, without allocating.
Span<char> buffer = stackalloc char[userInput.Length];
int written = userInput.AsSpan().ToUpperOrdinal(buffer);
Console.WriteLine($"Span-based ToUpperOrdinal:          {new string(buffer[..written])}");
