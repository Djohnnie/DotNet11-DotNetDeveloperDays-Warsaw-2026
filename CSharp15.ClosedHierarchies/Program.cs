// C# 15 - Closed hierarchies.
// Introduced: .NET 11 Preview 7 (C# 15 preview language version).
//
// The `closed` modifier tells the compiler that every derived type of an abstract
// class (or every implementation of an interface) lives in the same file/assembly.
// Because the compiler knows the full set of subtypes, a `switch` over a closed
// hierarchy is exhaustive without a discard arm, and forgetting a case is a warning.
//
// Source: https://github.com/dotnet/core/blob/main/release-notes/11.0/preview/preview7/csharp.md

using System.Globalization;

Shape[] shapes =
[
    new Circle(2.0),
    new Rectangle(3.0, 4.0),
    new Triangle(3.0, 4.0, 5.0),
];

foreach (Shape shape in shapes)
{
    Console.WriteLine($"{Describe(shape),-28} area = {Area(shape).ToString("F3", CultureInfo.InvariantCulture)}");
}

Console.WriteLine();
Console.WriteLine("Every arm is covered, so no default/discard case is required.");

// No `_` arm: the compiler proves exhaustiveness from the closed hierarchy.
static double Area(Shape shape) => shape switch
{
    Circle c => Math.PI * c.Radius * c.Radius,
    Rectangle r => r.Width * r.Height,
    Triangle t => HeronArea(t),
};

static string Describe(Shape shape) => shape switch
{
    Circle c => $"Circle(r={c.Radius})",
    Rectangle r => $"Rectangle({r.Width}x{r.Height})",
    Triangle t => $"Triangle({t.A},{t.B},{t.C})",
};

static double HeronArea(Triangle t)
{
    double s = (t.A + t.B + t.C) / 2.0;
    return Math.Sqrt(s * (s - t.A) * (s - t.B) * (s - t.C));
}

// `closed` restricts derivation to this file, which makes the switches above exhaustive.
// `closed` already implies abstract, so the modifier must not be repeated.
internal closed record Shape;

internal sealed record Circle(double Radius) : Shape;

internal sealed record Rectangle(double Width, double Height) : Shape;

internal sealed record Triangle(double A, double B, double C) : Shape;
