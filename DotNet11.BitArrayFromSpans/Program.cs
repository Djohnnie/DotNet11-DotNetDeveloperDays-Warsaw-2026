// .NET 11 Libraries - Construct bit arrays from spans.
// Introduced: RC 1.
//
// BitArray now accepts ReadOnlySpan<bool>, ReadOnlySpan<byte> and ReadOnlySpan<int>, so a bit
// array can be built directly from a slice or from stack-allocated data without first
// allocating an array. Windows Arm64 benchmarks showed 33-51% faster construction and
// 50-89% fewer allocations compared to calling span.ToArray().
//
// Source: https://github.com/dotnet/core/blob/main/release-notes/11.0/preview/rc1/libraries.md#construct-bitarray-values-from-spans

using System.Collections;

Console.WriteLine("== From ReadOnlySpan<byte> (stack allocated, zero heap input) ==");
Span<byte> bytes = stackalloc byte[] { 0b_0000_0011, 0b_1000_0000 };
var fromBytes = new BitArray(bytes);
Console.WriteLine($"length    : {fromBytes.Length}");
Console.WriteLine($"bit  0    : {fromBytes[0]}");
Console.WriteLine($"bit  1    : {fromBytes[1]}");
Console.WriteLine($"bit 15    : {fromBytes[15]}");
Console.WriteLine($"bits      : {Render(fromBytes)}");

Console.WriteLine();
Console.WriteLine("== From ReadOnlySpan<bool> ==");
Span<bool> flags = stackalloc bool[] { true, false, true, true, false };
var fromBools = new BitArray(flags);
Console.WriteLine($"bits      : {Render(fromBools)}");

Console.WriteLine();
Console.WriteLine("== From ReadOnlySpan<int> ==");
Span<int> words = stackalloc int[] { 0b_1010, 1 };
var fromInts = new BitArray(words);
Console.WriteLine($"length    : {fromInts.Length}");
Console.WriteLine($"bit 32    : {fromInts[32]}");

Console.WriteLine();
Console.WriteLine("== Slicing a larger buffer without ToArray() ==");
byte[] packet = [0xAA, 0xFF, 0x00, 0x0F, 0xF0];
var flagsSection = new BitArray(packet.AsSpan(1, 2));
Console.WriteLine($"slice bits: {Render(flagsSection)}");

static string Render(BitArray bits)
{
    char[] chars = new char[bits.Length];
    for (int i = 0; i < bits.Length; i++)
    {
        chars[i] = bits[i] ? '1' : '0';
    }

    return new string(chars);
}
