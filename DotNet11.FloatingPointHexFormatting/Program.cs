using System.Globalization;

// .NET 11 adds a hexadecimal IEEE-754 format for double, float and Half via the "X" format specifier.
// Formatting a floating-point value as decimal text ("R", "G17", ...) can still lose the exact bit
// pattern for some values once it's parsed back. The hex form preserves every bit, so round-tripping
// through it is always exact - useful when persisting or transmitting floating-point values that must
// come back byte-for-byte identical.

double value = Math.PI;
string decimalText = value.ToString("G17", CultureInfo.InvariantCulture);
string hexText = value.ToString("X", CultureInfo.InvariantCulture);

Console.WriteLine($"Original:        {value:R}");
Console.WriteLine($"Decimal (G17):   {decimalText}");
Console.WriteLine($"Hex (X):         {hexText}");

double roundTripped = double.Parse(hexText, NumberStyles.HexFloat, CultureInfo.InvariantCulture);
Console.WriteLine($"Parsed back:     {roundTripped:R}");
Console.WriteLine($"Exact round-trip: {roundTripped == value}");

// The same "X" specifier and NumberStyles.HexFloat work for float and Half too.
float single = MathF.E;
string singleHex = single.ToString("X", CultureInfo.InvariantCulture);
float singleRoundTripped = float.Parse(singleHex, NumberStyles.HexFloat, CultureInfo.InvariantCulture);
Console.WriteLine($"float e -> {singleHex} -> {singleRoundTripped}, exact: {singleRoundTripped == single}");
