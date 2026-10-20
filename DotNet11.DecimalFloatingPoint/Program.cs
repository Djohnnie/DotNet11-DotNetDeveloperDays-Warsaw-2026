using System.Globalization;
using System.Numerics;
using System.Runtime.CompilerServices;

// .NET 11 adds IEEE 754-2019 decimal floating-point types to System.Numerics.
// These complement System.Decimal; they do not change the meaning of decimal or the m suffix.
// Source: https://github.com/dotnet/core/blob/main/release-notes/11.0/preview/preview7/libraries.md#ieee-754-decimal-floating-point-types

CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;

Console.WriteLine("== Why add more decimal types to the BCL? ==");
Console.WriteLine("Standard IEEE decimal formats and interoperability, without a third-party numeric library.");
Console.WriteLine("Choose 32, 64, or 128 bits to trade storage for decimal precision and exponent range.");
Console.WriteLine("Get IEEE infinities/NaN and decimal floating-point generic math, not just finite decimal arithmetic.");
Console.WriteLine("They complement decimal; they are not automatically faster or better for every money calculation.");

Console.WriteLine();
Console.WriteLine("== Storage, significant decimal digits, and rounding ==");
Console.WriteLine($"{"Type",-12} {"Bits",4} {"Digits",6}  1 / 3");
ShowPrecision<decimal>("28-29");
ShowPrecision<Decimal32>("7");
ShowPrecision<Decimal64>("16");
ShowPrecision<Decimal128>("34");
Console.WriteLine("Base 10 does not mean unlimited precision: 1/3 still needs rounding.");

Console.WriteLine();
Console.WriteLine("== Base-10 arithmetic is not new: decimal already has it ==");
Console.WriteLine($"double    0.1 + 0.2 = {0.1 + 0.2:R}");
Console.WriteLine($"decimal   0.1 + 0.2 = {0.1m + 0.2m}");
Console.WriteLine($"Decimal32 0.1 + 0.2 = {(Decimal32)0.1m + (Decimal32)0.2m}");
Console.WriteLine($"Decimal64 0.1 + 0.2 = {(Decimal64)0.1m + (Decimal64)0.2m}");
Console.WriteLine($"Decimal128 0.1 + 0.2 = {(Decimal128)0.1m + (Decimal128)0.2m}");

Console.WriteLine();
Console.WriteLine("== Existing System.Decimal: finite values and a bounded scale ==");
Console.WriteLine($"decimal.MaxValue: {decimal.MaxValue}");
Console.WriteLine($"Can decimal represent 1e50? {decimal.TryParse("1e50", NumberStyles.Float, CultureInfo.InvariantCulture, out _)}");
Console.WriteLine($"decimal.Parse(\"1e-40\"): {decimal.Parse("1e-40", NumberStyles.Float, CultureInfo.InvariantCulture)}");

decimal one = 1m;
decimal zero = 0m;
try
{
    Console.WriteLine($"decimal 1 / 0: {one / zero}");
}
catch (DivideByZeroException exception)
{
    Console.WriteLine($"decimal 1 / 0 throws {exception.GetType().Name}; decimal has no infinity or NaN.");
}

ShowIeeeDecimal<Decimal32>();
ShowIeeeDecimal<Decimal64>();
ShowIeeeDecimal<Decimal128>();

Console.WriteLine();
Console.WriteLine("Keep decimal for existing monetary/domain contracts that need its established behavior.");
Console.WriteLine("Choose an IEEE decimal type when its standard format, precision/range, or IEEE math is required.");
Console.WriteLine("Parse decimal text directly or convert from decimal; avoid a binary double intermediate.");

static void ShowPrecision<T>(string digits) where T : IFloatingPoint<T>
{
    T third = T.One / T.CreateChecked(3);
    Console.WriteLine($"{typeof(T).Name,-12} {Unsafe.SizeOf<T>() * 8,4} {digits,6}  {third}");
}

static void ShowIeeeDecimal<T>() where T : IDecimalFloatingPointIeee754<T>, IMinMaxValue<T>
{
    Console.WriteLine();
    Console.WriteLine($"== {typeof(T).Name}: IEEE decimal floating point ==");
    Console.WriteLine($"MaxValue: {T.MaxValue}");
    Console.WriteLine($"1e50: {T.Parse("1e50", CultureInfo.InvariantCulture)}");
    Console.WriteLine($"1e-40: {T.Parse("1e-40", CultureInfo.InvariantCulture)}");
    Console.WriteLine($"1 / 0: {T.One / T.Zero}");
    Console.WriteLine($"0 / 0: {T.Zero / T.Zero}");
    Console.WriteLine($"IsNaN(0 / 0): {T.IsNaN(T.Zero / T.Zero)}");
    Console.WriteLine($"Sqrt(2), approximate: {T.Sqrt(T.CreateChecked(2))}");
}
