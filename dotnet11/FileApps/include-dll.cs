// Since .NET 11 Preview 6, the same #:include directive used for source files also accepts
// a path to an already-built .dll: file-based apps can reference a prebuilt assembly directly,
// no #:package or full project reference required. The referenced .dll is added as a plain
// assembly Reference based on its file extension.
//
// The referenced assembly below is produced by the small class library in _HelperLib/.
// Build it once, then run this file:
// dotnet build _HelperLib
// dotnet run include-dll.cs

#:include _HelperLib/bin/Debug/net11.0/FileApps.Helper.dll

Console.WriteLine(FileApps.Helper.MathHelper.Square(9));
