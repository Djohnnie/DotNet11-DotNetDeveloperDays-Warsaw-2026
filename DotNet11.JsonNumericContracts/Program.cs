// .NET 11 Libraries - Expand JSON contracts and numeric
// support.
// Introduced: Preview 1 (BFloat16); Preview 3 (naming and
// ignore policies); Preview 7
// (closed-hierarchy inference and decimal types); RC 1 (numeric
// converters + schema metadata).
//
// System.Text.Json gains built-in converters for BFloat16 and
// the IEEE 754 decimal
// floating-point types (Decimal32/Decimal64/Decimal128), and
// JsonSchemaExporter now
// describes byte data with "contentEncoding": "base64".
//
// Source:
// https://github.com/dotnet/core/blob/main/release-notes/11.0/preview/rc1/libraries.md#json-support-for-new-numeric-types-and-binary-schemas

using System.Numerics;
using System.Text.Json;
using System.Text.Json.Schema;

Console.WriteLine("== New numeric converters ==");

var measurement = new Measurement(
    Voltage: (Decimal64)1.229m,
    Current: (Decimal32)0.5m,
    Charge: (Decimal128)12345.6789m,
    Weight: (BFloat16)3.25f);

string json = JsonSerializer.Serialize(measurement);
Console.WriteLine(json);

Measurement roundTripped = JsonSerializer.Deserialize<Measurement>(json);
Console.WriteLine($"round-tripped equal: {roundTripped == measurement}");

Console.WriteLine();
Console.WriteLine("== JsonSchemaExporter reports base64 for binary data ==");

var options = JsonSerializerOptions.Default;
Console.WriteLine($"byte[]               : {options.GetJsonSchemaAsNode(typeof(byte[])).ToJsonString()}");
Console.WriteLine($"ReadOnlyMemory<byte> : {options.GetJsonSchemaAsNode(typeof(ReadOnlyMemory<byte>)).ToJsonString()}");

Console.WriteLine();
Console.WriteLine("== Naming and ignore policies ==");

var snakeCase = new JsonSerializerOptions
{
    PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
    WriteIndented = true,
};

Console.WriteLine(JsonSerializer.Serialize(measurement, snakeCase));

internal readonly record struct Measurement(
    Decimal64 Voltage,
    Decimal32 Current,
    Decimal128 Charge,
    BFloat16 Weight);
