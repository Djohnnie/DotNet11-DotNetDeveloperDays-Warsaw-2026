using System.Numerics;

// .NET 11 introduces System.Numerics.BFloat16, a 16-bit "brain float" floating-point type.
// Unlike System.Half (which trades range for precision), BFloat16 keeps the same 8-bit exponent
// as float/double (so it has the same huge dynamic range) but only 7 bits of mantissa.
// That makes it lossy for everyday math, but it's exactly the trade-off modern AI/ML workloads
// want: tensors that are half the size of float32 with no risk of overflow/underflow.
// BFloat16 implements the same generic math interfaces (INumber<T>, IFloatingPoint<T>, ...) as
// float and double, so it plugs straight into generic numeric code.

var a = (BFloat16)1.5f;
var b = (BFloat16)2.25f;

Console.WriteLine($"a = {a}, b = {b}");
Console.WriteLine($"a + b = {a + b}");
Console.WriteLine($"a * b = {a * b}");
Console.WriteLine($"Min/Max: {BFloat16.MinValue} .. {BFloat16.MaxValue}");
Console.WriteLine($"Epsilon: {BFloat16.Epsilon}");

// Round-tripping through float shows the precision loss BFloat16 accepts in exchange for
// keeping float's full exponent range: only ~7-8 significant bits survive the conversion.
float precise = 1.0f / 3.0f;
BFloat16 lossy = (BFloat16)precise;
float roundTripped = (float)lossy;
Console.WriteLine($"1/3 as float:    {precise:R}");
Console.WriteLine($"1/3 as BFloat16: {roundTripped:R} (precision lost, range preserved)");

// Generic math: the same helper works for BFloat16, float, double, Half, ...
static T Average<T>(T x, T y) where T : IFloatingPoint<T> => (x + y) / (T.One + T.One);
Console.WriteLine($"Average(a, b) via generic math = {Average(a, b)}");



Decimal d = 1m / 3m;
Decimal32 d32 = (Decimal32)1 / (Decimal32)3;
Decimal64 d64 = (Decimal64)1 / (Decimal64)3;
Decimal128 d128 = (Decimal128)1 / (Decimal128)3;