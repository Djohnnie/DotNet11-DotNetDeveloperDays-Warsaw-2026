using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace RuntimeAsyncDemo;

// Both projects compile this file so ILSpy compares the lowering of identical source.
public static class Program
{
    public static void Main()
    {
        Console.WriteLine(RuntimeInformation.FrameworkDescription);
        Console.WriteLine($"Result: {OuterAsync().GetAwaiter().GetResult()}");
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    public static async Task<int> OuterAsync()
    {
        int value = await MiddleAsync(20);
        return value * 2;
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    public static async Task<int> MiddleAsync(int input)
    {
        int value = await InnerAsync(input);
        return value + 1;
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    public static async Task<int> InnerAsync(int input)
    {
        // Capture the live call chain before yielding; exception traces hide this difference.
        Console.WriteLine(new StackTrace(fNeedFileInfo: true));
        await Task.Yield();
        return input;
    }
}
