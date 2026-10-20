// .NET 11 Libraries - Run processes and capture output.
// Introduced: Preview 4.
//
// `Process.Run`, `Process.RunAsync`,
// `Process.RunAndCaptureText` and
// `Process.RunAndCaptureTextAsync` are one-shot APIs for
// launching a child process and
// getting its result, without manually wiring up
// OutputDataReceived/ErrorDataReceived.
// `Process.ReadAllTextAsync` / `ReadAllLinesAsync` give a
// single-call read of child output,
// and `ProcessOutputLine` lets callers tell stdout from stderr
// without string parsing.
//
// Source:
// https://github.com/dotnet/core/blob/main/release-notes/11.0/preview/preview4/libraries.md

using System.Diagnostics;

string shell = OperatingSystem.IsWindows() ? "cmd" : "/bin/sh";
string[] echoArgs = OperatingSystem.IsWindows()
    ? ["/c", "echo hello from a child process"]
    : ["-c", "echo hello from a child process"];

Console.WriteLine("== RunAndCaptureTextAsync ==");
ProcessTextOutput captured = await Process.RunAndCaptureTextAsync(shell, echoArgs);
Console.WriteLine($"stdout   : {captured.StandardOutput.TrimEnd()}");
Console.WriteLine($"stderr   : '{captured.StandardError.TrimEnd()}'");
Console.WriteLine($"exit code: {captured.ExitStatus.ExitCode}");

Console.WriteLine();
Console.WriteLine("== ReadAllTextAsync on a started process ==");
using (var process = Process.Start(new ProcessStartInfo("dotnet", ["--version"])
{
    RedirectStandardOutput = true,
    RedirectStandardError = true,
})!)
{
    (string stdout, string stderr) = await process.ReadAllTextAsync();
    Console.WriteLine($"dotnet --version -> {stdout.TrimEnd()}");
    Console.WriteLine($"stderr           -> '{stderr.TrimEnd()}'");
}

Console.WriteLine();
Console.WriteLine("== ReadAllLinesAsync (stdout and stderr are distinguishable) ==");
string[] mixedArgs = OperatingSystem.IsWindows()
    ? ["/c", "echo to-stdout& echo to-stderr 1>&2"]
    : ["-c", "echo to-stdout; echo to-stderr 1>&2"];

using (var process = Process.Start(new ProcessStartInfo(shell, mixedArgs)
{
    RedirectStandardOutput = true,
    RedirectStandardError = true,
})!)
{
    await foreach (ProcessOutputLine line in process.ReadAllLinesAsync())
    {
        string stream = line.StandardError ? "stderr" : "stdout";
        Console.WriteLine($"[{stream}] {line.Content}");
    }
}

Console.WriteLine();
Console.WriteLine("== RunAsync (no capture, just the exit status) ==");
ProcessExitStatus status = await Process.RunAsync(shell, echoArgs);
Console.WriteLine($"exited with {status.ExitCode}");
