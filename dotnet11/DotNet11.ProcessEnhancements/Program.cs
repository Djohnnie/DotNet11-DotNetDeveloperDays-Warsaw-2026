using System.Diagnostics;

// System.Diagnostics.Process received its biggest update in years in .NET 11.
// Two of the additions:
// - ProcessStartInfo.StartSuspended (Windows) starts a process in a suspended state, so you can attach
//   diagnostics, set its priority/affinity, or otherwise prepare it before it runs any of its own code.
//   SafeProcessHandle.Resume then lets it actually start executing.
// - Process.TryGetProcessById returns false instead of throwing when no process with the given id exists,
//   avoiding a try/catch just to check whether a process is still running.

var startInfo = new ProcessStartInfo("cmd.exe", "/c ver")
{
    StartSuspended = true,
    RedirectStandardOutput = true,
    UseShellExecute = false,
};

using var process = Process.Start(startInfo)!;
Console.WriteLine($"Started process {process.Id}, suspended: {startInfo.StartSuspended}");

// The process exists but hasn't run any code yet - do any setup you need here.
Console.WriteLine("Resuming the suspended process...");
process.SafeHandle.Resume();

var output = process.StandardOutput.ReadToEnd();
process.WaitForExit();
Console.WriteLine($"Process output: {output.Trim()}");

// TryGetProcessById avoids a try/catch around GetProcessById for a process that may have already exited.
if (Process.TryGetProcessById(process.Id, out var stillRunning))
{
    Console.WriteLine($"Process {process.Id} is still tracked: {stillRunning.ProcessName}");
}
else
{
    Console.WriteLine($"Process {process.Id} has already exited.");
}
