// Requires the .NET 11 Preview 7 SDK or later - this API isn't present yet in Preview 6.
//
// .NET 11 introduces System.Numerics.Complex<T>, a generic complex number type that mirrors the full
// public surface of the existing (double-only) System.Numerics.Complex - arithmetic operators, Abs,
// Conjugate, Reciprocal, Log/Exp/Sqrt/Pow, the trig/hyperbolic families, parsing/formatting, and
// INumberBase<Complex<T>>. It works with any T : IFloatingPointIeee754<T>, IMinMaxValue<T>, so the same
// code can run in float, double, or Half precision.

using System.Numerics;

Complex<float> a = new(3, 4);

Complex<Decimal128> b = new(1, 2);



Console.WriteLine($"a = {a}, b = {b}");
Console.WriteLine($"a + b = {a + b}");
Console.WriteLine($"a * b = {a * b}");
Console.WriteLine($"|a| (Abs) = {Complex<float>.Abs(a)}");
Console.WriteLine($"conjugate(a) = {Complex<float>.Conjugate(a)}");
Console.WriteLine($"sqrt(a) = {Complex<float>.Sqrt(a)}");

// T x Complex<T> and Complex<T> x T overloads let you scale without an explicit conversion.
Complex<float> scaled = 2f * a;
Console.WriteLine($"2 * a = {scaled}");

// Interop with the original, double-precision Complex via the generic-math conversion APIs.
Complex<double> fromNonGeneric = Complex<double>.CreateSaturating(new Complex(1, 1));
Console.WriteLine($"Complex<double> from Complex(1, 1) = {fromNonGeneric}");
