// "Unsafe Evolution" (C# 15, still Preview as of .NET 11 Preview 5-7) refines the boundary between
// "code that merely mentions a pointer" and "code that actually dereferences unmanaged memory",
// so the `unsafe` keyword is only required where real memory-safety risk exists.
//
// https://github.com/dotnet/core/blob/main/release-notes/11.0/preview/preview5/csharp.md#unsafe-evolution
// https://github.com/dotnet/core/blob/main/release-notes/11.0/preview/preview7/csharp.md#unsafe-evolution-compat-mode-and-nameof

// 1) A pointer DECLARATION and the address-of operator (&) no longer require an `unsafe` context -
//    only DEREFERENCING the pointer (*pointer) does. Before this, the whole block below would have
//    needed to be wrapped in `unsafe { ... }`.
int value = 42;
int* pointer = &value;

unsafe
{
    Console.WriteLine($"*pointer = {*pointer}");
}

// 2) nameof(...) referencing a "requires-unsafe" member no longer needs an unsafe context either,
//    matching how nameof already worked for every other kind of member. Calling PeekFirstByte(...)
//    for real still requires `unsafe`, but just naming it for logging/reflection/nameof(...) purposes
//    does not:
Console.WriteLine($"Member name via nameof (no unsafe needed): {nameof(UnsafeBufferReader.PeekFirstByte)}");

byte[] buffer = [0xFF, 0x00, 0x01];
unsafe
{
    Console.WriteLine($"Actually calling {nameof(UnsafeBufferReader.PeekFirstByte)}: 0x{UnsafeBufferReader.PeekFirstByte(buffer):X2}");
}

static class UnsafeBufferReader
{
    public static unsafe byte PeekFirstByte(byte[] buffer)
    {
        fixed (byte* p = buffer)
        {
            return *p;
        }
    }
}
