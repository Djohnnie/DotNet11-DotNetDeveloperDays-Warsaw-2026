using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace RuntimeAsyncDemo;

// Both projects compile this file so ILSpy compares the
// lowering of identical source.
// With runtime-async=off, the compiler rewrites each async
// method into a state
// machine with MoveNext(), an async method builder, and
// awaiter/state fields.
// With runtime-async=on, the runtime/JIT handles suspension and
// resumption instead.
// Benefits: less compiler-generated IL, cleaner live stack
// traces, and opportunities
// for lower allocation and continuation overhead because the
// JIT understands await.
// This is not a guarantee of zero allocations or faster I/O;
// gains depend on the workload.
//
// Here, inspect the stack printed inside InnerAsync: runtime
// async removes much of
// the MoveNext()/builder plumbing from the live OuterAsync ->
// MiddleAsync -> InnerAsync
// call chain. NoInlining keeps those calls visible.
// Task.Yield() exercises suspension;
// both versions still return 42. This demonstrates
// implementation differences, not a benchmark.
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
        // Capture the live call chain before yielding;
        // exception traces hide this difference.
        Console.WriteLine(new StackTrace(fNeedFileInfo: true));
        await Task.Yield();
        return input;
    }
}
