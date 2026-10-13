// .NET 11 Libraries - Parse numbers from delimited input.
// Introduced: Preview 7.
//
// INumberBase<TSelf>.TryParsePartial adds generic numeric parsing overloads that report how
// much of the input was consumed. Parsers for formats such as CSV can parse one numeric field
// and continue with the remaining input, without copying it and without treating the
// delimiter as a parse failure.
//
// Source: https://github.com/dotnet/core/blob/main/release-notes/11.0/preview/preview7/libraries.md#partial-numeric-parsing

using System.Globalization;

Console.WriteLine("== The release-note example ==");
ReadOnlySpan<char> input = "123; 456";
if (int.TryParsePartial(input, NumberStyles.Integer, CultureInfo.InvariantCulture, out int value, out int charsConsumed))
{
    Console.WriteLine($"value     : {value}");
    input = input[charsConsumed..];
    Console.WriteLine($"remaining : '{input}'");
}

Console.WriteLine();
Console.WriteLine("== Tokenizing a CSV row with no allocations ==");
foreach (double number in ParseDelimited<double>("1.5,2.25,3.75,-4", ','))
{
    Console.WriteLine($"  {number.ToString(CultureInfo.InvariantCulture)}");
}

Console.WriteLine();
Console.WriteLine("== Works for any INumberBase<T> ==");
Console.WriteLine($"decimal : {string.Join(", ", ParseDelimited<decimal>("10.01;20.02;30.03", ';'))}");
Console.WriteLine($"long    : {string.Join(", ", ParseDelimited<long>("9|8|7", '|'))}");

Console.WriteLine();
Console.WriteLine("== Trailing junk is reported, not swallowed ==");
ReadOnlySpan<char> mixed = "42abc";
if (int.TryParsePartial(mixed, NumberStyles.Integer, CultureInfo.InvariantCulture, out int parsed, out int consumed))
{
    Console.WriteLine($"parsed {parsed} from {consumed} char(s); leftover '{mixed[consumed..]}'");
}

static List<T> ParseDelimited<T>(ReadOnlySpan<char> text, char delimiter)
    where T : System.Numerics.INumberBase<T>
{
    var results = new List<T>();

    while (!text.IsEmpty)
    {
        if (!T.TryParsePartial(text, NumberStyles.Number, CultureInfo.InvariantCulture, out T? number, out int used))
        {
            break;
        }

        results.Add(number);
        text = text[used..];

        if (!text.IsEmpty && text[0] == delimiter)
        {
            text = text[1..];
        }
    }

    return results;
}
