// Beginning with .NET 11, "dotnet run" accepts an -e/--environment-variable option
// to pass environment variables straight from the command line, without creating a
// launchSettings.json profile just to test a value.
//
// Run it with:
// dotnet run -e GREETING="Hello from .NET 11" env-vars.cs

var greeting = Environment.GetEnvironmentVariable("GREETING");

Console.WriteLine(greeting is null
    ? "GREETING is not set - try: dotnet run -e GREETING=\"Hello from .NET 11\" env-vars.cs"
    : $"GREETING = {greeting}");
