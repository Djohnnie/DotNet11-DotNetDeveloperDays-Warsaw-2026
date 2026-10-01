// Beginning with .NET 11, file-based apps (dotnet run app.cs) can be split across multiple files.
// The #:include directive pulls in another C# source file from the same file-based app,
// so shared helpers no longer force you to graduate to a full csproj-based project.
// Directives inside an included file are processed transitively, and the included file
// keeps ordinary top-level type/member declarations (it must not itself be an entry point).
//
// Run it with:
// dotnet run greeter.cs

#:include Formatting.cs

Console.WriteLine(Formatting.Shout("hello from a multi-file file-based app"));
