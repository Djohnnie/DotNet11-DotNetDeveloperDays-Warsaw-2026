// Helper file included by greeter.cs via "#:include Formatting.cs".
// This file is not itself an entry point - it just contributes ordinary declarations
// to the same compilation, exactly like an extra file in a real project would.

static class Formatting
{
    public static string Shout(string message) => message.ToUpperInvariant() + "!";
}
