using System.Text;

// A Rune represents a single Unicode scalar value. Characters outside the Basic Multilingual
// Plane (most emoji, some CJK ideographs, ...) don't fit in one System.Char - they're encoded
// as a surrogate *pair* of two chars. Code that naively iterates char-by-char can split such a
// pair in half and corrupt the text. .NET 11 rounds out Rune support by adding Rune-aware
// overloads directly on String, StringBuilder and TextWriter, so you no longer need to convert
// through intermediate collections just to work correctly with the full Unicode range.

const string text = "Warsaw \U0001F1F5\U0001F1F1 .NET 11 \U0001F680"; // flag + rocket are surrogate pairs

Console.WriteLine($"\"{text}\"");
Console.WriteLine($"UTF-16 char count: {text.Length}");
Console.WriteLine($"Unicode scalar (Rune) count: {text.EnumerateRunes().Count()}");

Console.WriteLine();
Console.WriteLine("Naive char-by-char iteration corrupts surrogate pairs:");
foreach (var c in text.Where(c => !char.IsWhiteSpace(c) && !char.IsAscii(c)))
{
    Console.WriteLine($"  char U+{(int)c:X4} (lone surrogate half, not a real character)");
}

Console.WriteLine();
Console.WriteLine("Rune-based iteration handles them correctly:");
foreach (var rune in text.EnumerateRunes().Where(r => !Rune.IsWhiteSpace(r) && !r.IsAscii))
{
    Console.WriteLine($"  Rune U+{rune.Value:X4} '{rune}' (Utf16SequenceLength: {rune.Utf16SequenceLength})");
}

// New in .NET 11: StringBuilder and TextWriter gain first-class Rune overloads.
var builder = new StringBuilder();
foreach (var rune in text.EnumerateRunes())
{
    builder.Append(rune);
}
Console.WriteLine();
Console.WriteLine($"Rebuilt via StringBuilder.Append(Rune): \"{builder}\" (matches original: {builder.ToString() == text})");

Console.Out.Write("Written straight to the console via TextWriter.Write(Rune): ");
foreach (var rune in "\U0001F680\U0001F1F5\U0001F1F1".EnumerateRunes())
{
    Console.Out.Write(rune);
}
Console.WriteLine();
