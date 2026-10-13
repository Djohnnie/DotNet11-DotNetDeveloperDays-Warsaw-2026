# .NET 11 and C# 15 features

This guide expands the feature examples in this directory with short
explanations and code excerpts adapted from the linked release notes and
documentation. It focuses on user-facing features rather than listing every
bug fix, implementation detail, or platform-specific change. .NET 11 and C# 15
are still in preview/RC: check the linked source for availability and API
changes in the SDK version you use.

## Contents

- [C# 15](#c-15)
  - [Collection expression arguments](#collection-expression-arguments)
  - [Union types](#union-types)
  - [Closed hierarchies](#closed-hierarchies)
  - [Extension indexers](#extension-indexers)
  - [Labeled `break` and `continue`](#labeled-break-and-continue)
  - [Memory-safety updates](#memory-safety-updates)
- [.NET libraries and runtime](#net-libraries-and-runtime)
  - [Run processes and capture output](#run-processes-and-capture-output)
  - [Control process lifetime and inspect exit status](#control-process-lifetime-and-inspect-exit-status)
  - [Stream memory and text without copying](#stream-memory-and-text-without-copying)
  - [Compress and decompress spans](#compress-and-decompress-spans)
  - [Protect ZIP entries with passwords](#protect-zip-entries-with-passwords)
  - [Add Zstandard compression](#add-zstandard-compression)
  - [Validate ZIP checksums when reading](#validate-zip-checksums-when-reading)
  - [Create hard links](#create-hard-links)
  - [Serialize asynchronous streams as JSON Lines](#serialize-asynchronous-streams-as-json-lines)
  - [Expand JSON contracts and numeric support](#expand-json-contracts-and-numeric-support)
  - [Resolve DNS record types](#resolve-dns-record-types)
  - [Work with Unicode scalars and validate text](#work-with-unicode-scalars-and-validate-text)
  - [Use ordinal casing and floating-point hex](#use-ordinal-casing-and-floating-point-hex)
  - [Construct bit arrays from spans](#construct-bit-arrays-from-spans)
  - [Build frozen dictionaries from collection expressions](#build-frozen-dictionaries-from-collection-expressions)
  - [Add generic numeric APIs](#add-generic-numeric-apis)
  - [Parse numbers from delimited input](#parse-numbers-from-delimited-input)
  - [Join both sides of two LINQ sequences](#join-both-sides-of-two-linq-sequences)
  - [Validate data asynchronously](#validate-data-asynchronously)
  - [Use newer cryptography and faster `Half` operations](#use-newer-cryptography-and-faster-half-operations)
  - [Runtime-managed async (preview)](#runtime-managed-async-preview)
- [SDK and build](#sdk-and-build)
  - [Run the CLI and file-based apps more efficiently](#run-the-cli-and-file-based-apps-more-efficiently)
  - [Pass environment variables and use improved watch workflows](#pass-environment-variables-and-use-improved-watch-workflows)
  - [Improve test runs](#improve-test-runs)
  - [Publish reproducible containers](#publish-reproducible-containers)
  - [Add tar support and expose MSBuild globs](#add-tar-support-and-expose-msbuild-globs)
- [ASP.NET Core and Blazor](#aspnet-core-and-blazor)
  - [Protect endpoints from cross-origin requests by default](#protect-endpoints-from-cross-origin-requests-by-default)
  - [Use async validation in Minimal APIs](#use-async-validation-in-minimal-apis)
  - [Short-circuit selected endpoints](#short-circuit-selected-endpoints)
  - [Describe server-sent events in OpenAPI](#describe-server-sent-events-in-openapi)
  - [Navigate relatively and carry temporary data in Blazor](#navigate-relatively-and-carry-temporary-data-in-blazor)
  - [Pause inactive Blazor Server circuits](#pause-inactive-blazor-server-circuits)
  - [Cache server-rendered component subtrees](#cache-server-rendered-component-subtrees)
  - [Improve QuickGrid interaction](#improve-quickgrid-interaction)
  - [Add environment and metadata-aware UI](#add-environment-and-metadata-aware-ui)
- [Entity Framework Core 11](#entity-framework-core-11)
  - [Use complex types with more mappings](#use-complex-types-with-more-mappings)
  - [Translate more LINQ and generate leaner SQL](#translate-more-linq-and-generate-leaner-sql)
  - [Query JSON and create JSON indexes](#query-json-and-create-json-indexes)
  - [Search SQL Server with vectors and full text](#search-sql-server-with-vectors-and-full-text)
  - [Simplify migration workflows](#simplify-migration-workflows)
  - [Inspect tracked entries without extra change detection](#inspect-tracked-entries-without-extra-change-detection)
  - [Warn when async queries run synchronously](#warn-when-async-queries-run-synchronously)
- [Windows Forms](#windows-forms)
  - [Opt into modern visual styles and avoid startup flashes](#opt-into-modern-visual-styles-and-avoid-startup-flashes)
  - [Render check boxes as toggle switches](#render-check-boxes-as-toggle-switches)
  - [Suspend painting during bulk updates](#suspend-painting-during-bulk-updates)
- [WPF](#wpf)
  - [Reliability and compatibility fixes](#reliability-and-compatibility-fixes)
- [Related sources](#related-sources)

## C# 15

### Collection expression arguments

**Introduced:** Preview 1.

A leading `with(...)` element passes constructor or factory arguments such as
capacity or a comparer into a collection expression.

```csharp
List<string> names = [with(capacity: 20), "Ada", "Grace"];
HashSet<string> tags =
    [with(StringComparer.OrdinalIgnoreCase), "dotnet", "DOTNET"];
```

[Source: What's new in C# 15](https://learn.microsoft.com/en-us/dotnet/csharp/whats-new/csharp-15#collection-expression-arguments)

### Union types

**Introduced:** Preview 2 (compiler support); Preview 5 (union declarations and patterns); Preview 6 (framework support types).

A union holds exactly one of its declared case types. Implicit conversions and
exhaustive switch checking make alternatives explicit to callers.

```csharp
public union Pet(Cat, Dog);

string Name(Pet pet) => pet switch
{
    Cat cat => cat.Name,
    Dog dog => dog.Name
};
```

[Source: What's new in C# 15](https://learn.microsoft.com/en-us/dotnet/csharp/whats-new/csharp-15#union-types)

### Closed hierarchies

**Introduced:** Preview 5.

The `closed` modifier restricts direct descendants to the declaring assembly,
so the compiler can check switches against the known set of cases.

```csharp
public closed record GateState;
public record Open : GateState;
public record Closed : GateState;
```

[Source: What's new in C# 15](https://learn.microsoft.com/en-us/dotnet/csharp/whats-new/csharp-15#closed-hierarchies)

### Extension indexers

**Introduced:** Preview 6.

An extension block can add an indexer to an existing receiver type, letting
callers use index syntax as though it were declared on that type.

```csharp
static class SequenceIndexer
{
    extension(IEnumerable<int> sequence)
    {
        public int this[int index] => sequence.ElementAt(index);
    }
}
```

[Source: What's new in C# 15](https://learn.microsoft.com/en-us/dotnet/csharp/whats-new/csharp-15#extension-indexers)

### Labeled `break` and `continue`

**Introduced:** Preview 7.

Labeled jumps target an enclosing loop or switch directly, avoiding flags or
`goto` when nested control flow needs to continue or exit an outer construct.

```csharp
outer: for (int row = 0; row < height; row++)
{
    for (int column = 0; column < width; column++)
        if (IsGoal(row, column)) break outer;
}
```

[Source: What's new in C# 15](https://learn.microsoft.com/en-us/dotnet/csharp/whats-new/csharp-15#labeled-break-and-continue)

### Memory-safety updates

**Introduced:** Preview 5; additional rules and refinements arrived in Previews 7 and RC 1.

The preview memory-safety model distinguishes pointer declarations from
operations that access pointed-to memory. Taking an address and pinning no
longer require an `unsafe` context; dereferencing still does. `unsafe(expr)`
marks a single unsafe expression.

```csharp
int value = 42;
int* pointer = &value; // declaration/address-taking doesn't need unsafe
int copy = unsafe(*pointer); // memory access remains explicitly unsafe
```

[Source: What's new in C# 15](https://learn.microsoft.com/en-us/dotnet/csharp/whats-new/csharp-15#memory-safety)

## .NET libraries and runtime

### Run processes and capture output

**Introduced:** Preview 4.

One-shot `Process` helpers start a child process and capture its output and
exit status without manually setting up output events.

```csharp
ProcessTextOutput result = await Process.RunAndCaptureTextAsync(
    "git", ["status", "--porcelain"]);
Console.WriteLine(result.StandardOutput);
Console.WriteLine(result.ExitStatus.ExitCode);
```

[Source: .NET 11 Preview 4 libraries](https://github.com/dotnet/core/blob/main/release-notes/11.0/preview/preview4/libraries.md#process-gets-a-major-api-expansion)

### Control process lifetime and inspect exit status

**Introduced:** Preview 4 (detached launches and handle controls); Preview 6 (suspended starts and process lookup); RC 1 (POSIX signals and exit status).

New APIs support detached and suspended starts, explicit POSIX signaling,
handle management, and exit results that distinguish normal exit from signal
termination.

```csharp
using Process process = Process.Start("sleep", "30")!;
process.Signal(PosixSignal.SIGTERM);
ProcessExitStatus status = await process.WaitForExitStatusAsync();
```

[Source: .NET 11 RC 1 libraries](https://github.com/dotnet/core/blob/main/release-notes/11.0/preview/rc1/libraries.md#signal-processes-and-inspect-termination-status)

### Stream memory and text without copying

**Introduced:** Preview 6.

`ReadOnlyMemoryStream`, `WritableMemoryStream`, `ReadOnlySequenceStream`, and
`StringStream` adapt existing in-memory data to APIs that require a `Stream`.
They avoid an intermediate `MemoryStream` and copied byte array.

```csharp
using Stream body = new ReadOnlyMemoryStream(payload);
await httpClient.PostAsync(uri, new StreamContent(body));
```

[Source: .NET 11 Preview 6 libraries](https://github.com/dotnet/core/blob/main/release-notes/11.0/preview/preview6/libraries.md#stream-adapters-for-memory-and-text)

### Compress and decompress spans

**Introduced:** Preview 4; codec reset/reuse arrived in RC 1.

Deflate, ZLib, and GZip codecs can operate directly on spans, avoiding stream
wrappers and allowing callers to manage buffers for one-shot or streaming work.
Encoder and decoder instances can also be reset and reused.

```csharp
using ZLibEncoder encoder = new();
OperationStatus status = encoder.Compress(
    source, destination, out int consumed, out int written,
    isFinalBlock: true);
```

[Source: .NET 11 Preview 4 libraries](https://github.com/dotnet/core/blob/main/release-notes/11.0/preview/preview4/libraries.md#span-based-deflate-zlib-and-gzip-encoderdecoder-apis)

### Protect ZIP entries with passwords

**Introduced:** Preview 7.

ZIP APIs can create and open password-protected entries with supported
encryption methods. The entry also exposes which encryption method it uses.

```csharp
ZipArchiveEntry entry = archive.CreateEntry(
    "notes.txt", password: password,
    encryptionMethod: ZipEncryptionMethod.Aes256);
using Stream content = entry.Open(password);
```

[Source: .NET 11 Preview 7 libraries](https://github.com/dotnet/core/blob/main/release-notes/11.0/preview/preview7/libraries.md#zip-archive-password-support)

### Add Zstandard compression

**Introduced:** Preview 1 for core compression; Preview 3 for the `System.IO.Compression` API placement; Preview 7 for compressed HTTP request content.

Zstandard is available alongside the other `System.IO.Compression` formats,
and HTTP content can be compressed with GZip, Brotli, or Zstandard wrappers.

```csharp
using var request = new HttpRequestMessage(HttpMethod.Post, uri)
{
    Content = new ZstandardCompressedContent(
        new StringContent(json), CompressionLevel.Optimal)
};
using HttpResponseMessage response = await client.SendAsync(request);
```

[Source: .NET 11 Preview 7 libraries](https://github.com/dotnet/core/blob/main/release-notes/11.0/preview/preview7/libraries.md#http-request-compression)

### Validate ZIP checksums when reading

**Introduced:** Preview 3.

ZIP entry reads now validate CRC32. Corrupted or truncated entry data fails
with `InvalidDataException` instead of being silently returned.

```csharp
using Stream entry = archive.GetEntry("data.bin")!.Open();
await entry.CopyToAsync(destination); // Invalid data can throw.
```

[Source: .NET 11 Preview 3 libraries](https://github.com/dotnet/core/blob/main/release-notes/11.0/preview/preview3/libraries.md#zstandard-moved-to-systemiocompression-and-zip-reads-validate-crc32)

### Create hard links

**Introduced:** Preview 1.

`File.CreateHardLink` and `FileInfo.CreateAsHardLink` create hard links, which
point at the same file data on disk. Unlike symbolic links, they don't usually
require administrator privileges on Windows, but they can't span volumes.

```csharp
FileSystemInfo link = File.CreateHardLink("./link.txt", "./original.txt");

var fileInfo = new FileInfo("./original.txt");
fileInfo.CreateAsHardLink("./link.txt");
```

[Source: .NET 11 Preview 1 libraries](https://github.com/dotnet/core/blob/main/release-notes/11.0/preview/preview1/libraries.md#hard-link-creation-apis)

### Serialize asynchronous streams as JSON Lines

**Introduced:** Preview 5.

`System.Text.Json` can write `IAsyncEnumerable<T>` as a JSON array or as
newline-delimited JSON. JSON Lines is useful for streaming records to logs and
consumers that process one JSON value per line.

```csharp
await JsonSerializer.SerializeAsyncEnumerable(
    stream, Readings(), topLevelValues: true);
```

[Source: .NET 11 Preview 5 libraries](https://github.com/dotnet/core/blob/main/release-notes/11.0/preview/preview5/libraries.md#systemtextjson-supports-json-lines-serialization)

### Expand JSON contracts and numeric support

**Introduced:** Preview 1 (BFloat16); Preview 3 (naming and ignore policies); Preview 5 (JSON Lines and F# unions); Preview 6 (C# union serialization); Preview 7 (closed-hierarchy inference and decimal types); RC 1 (additional numeric converters and schema metadata).

`System.Text.Json` adds built-in converters for `BFloat16` and IEEE decimal
floating-point types, more naming and ignore controls, improved source
generation, and closed-hierarchy/union support. JSON Schema export also
describes byte data as base64.

```csharp
string json = JsonSerializer.Serialize(
    new Measurement((Decimal64)1.229m));
Measurement? copy = JsonSerializer.Deserialize<Measurement>(json);
```

[Source: .NET 11 RC 1 libraries](https://github.com/dotnet/core/blob/main/release-notes/11.0/preview/rc1/libraries.md#json-support-for-new-numeric-types-and-binary-schemas)

### Resolve DNS record types

**Introduced:** Preview 7; Linux implementation arrived in RC 1.

Typed DNS APIs return record data together with response status and negative
cache information. The APIs cover address, SRV, MX, TXT, CNAME, PTR, and NS
records; platform support varies by preview.

```csharp
DnsResult<SrvRecord> result =
    await Dns.ResolveSrvAsync("_ldap._tcp.example.com");
foreach (SrvRecord record in result.Records)
    Console.WriteLine($"{record.Target}:{record.Port}");
```

[Source: .NET 11 Preview 7 libraries](https://github.com/dotnet/core/blob/main/release-notes/11.0/preview/preview7/libraries.md#dns-record-resolution-apis)

### Work with Unicode scalars and validate text

**Introduced:** Preview 1 (Rune-aware APIs); Preview 4 (UTF validity and malformed-subsequence detection).

String APIs accept `Rune` values for Unicode-aware search and manipulation.
Unicode helpers can also locate malformed UTF-8/UTF-16 sequences, which lets
parsers report a useful error position.

```csharp
bool hasSmile = text.Contains(new Rune(0x1F642));
int invalidAt = Utf8.IndexOfInvalidSubsequence(inputBytes);
```

[Source: .NET 11 libraries documentation](https://learn.microsoft.com/en-us/dotnet/core/whats-new/dotnet-11/libraries#string-and-character-enhancements)

### Use ordinal casing and floating-point hex

**Introduced:** Preview 4 (floating-point hex format); Preview 7 (ordinal casing).

Ordinal casing is deterministic and culture-independent. Floating-point
hexadecimal formatting and parsing preserves the exact IEEE-754 bit pattern.

```csharp
string key = userInput.ToUpperOrdinal();
double value = Math.PI;
double exactRoundTrip = double.Parse(
    value.ToString("X"), NumberStyles.HexFloat);
```

[Source: .NET 11 Preview 7 libraries](https://github.com/dotnet/core/blob/main/release-notes/11.0/preview/preview7/libraries.md#ordinal-casing-apis) · [Preview 4 floating-point hex](https://github.com/dotnet/core/blob/main/release-notes/11.0/preview/preview4/libraries.md#floating-point-hex-formatting-and-parsing)

### Construct bit arrays from spans

**Introduced:** RC 1.

`BitArray` accepts Boolean, byte, and integer spans, so slices and stack
allocations can be used without first copying into an array.

```csharp
Span<byte> bytes = stackalloc byte[] { 0b_0000_0011 };
var bits = new BitArray(bytes);
```

[Source: .NET 11 RC 1 libraries](https://github.com/dotnet/core/blob/main/release-notes/11.0/preview/rc1/libraries.md#construct-bitarray-values-from-spans)

### Build frozen dictionaries from collection expressions

**Introduced:** Preview 1.

`FrozenDictionary<TKey, TValue>` carries a `[CollectionBuilder]` attribute, so
it can be created from a C# dictionary expression. A `with(...)` argument
supplies the comparer, matching `FrozenSet` and `ImmutableDictionary`.

```csharp
FrozenDictionary<string, int> lookup =
    [with(StringComparer.Ordinal), "one": 1, "two": 2, "three": 3];
```

[Source: .NET 11 Preview 1 libraries](https://github.com/dotnet/core/blob/main/release-notes/11.0/preview/preview1/libraries.md#frozendictionary-collection-expression-support)

### Add generic numeric APIs

**Introduced:** Preview 1 (BFloat16); Preview 5 (generic random numbers); Preview 7 (decimal floating-point and `Complex<T>`).

Generic random-number methods and `Complex<T>` let algorithms work with
type-parameterized integer and floating-point types, including `Half` and
`BFloat16`.

```csharp
int roll = Random.Shared.NextInteger<int>(1, 7);
Complex<float> z = new(3f, 4f);
Console.WriteLine(z.GetMagnitude()); // 5
```

[Source: .NET 11 Preview 5 generic random APIs](https://github.com/dotnet/core/blob/main/release-notes/11.0/preview/preview5/libraries.md#random-adds-generic-numeric-apis) · [Preview 7 generic complex](https://github.com/dotnet/core/blob/main/release-notes/11.0/preview/preview7/libraries.md#generic-complext)

### Parse numbers from delimited input

**Introduced:** Preview 7.

`TryParsePartial` parses a numeric prefix and reports how many characters it
consumed, so a parser can continue at the delimiter without copying the input.

```csharp
ReadOnlySpan<char> input = "123; 456";
int.TryParsePartial(input, NumberStyles.Integer,
    CultureInfo.InvariantCulture, out int value, out int consumed);
input = input[consumed..]; // "; 456"
```

[Source: .NET 11 Preview 7 libraries](https://github.com/dotnet/core/blob/main/release-notes/11.0/preview/preview7/libraries.md#partial-numeric-parsing)

### Join both sides of two LINQ sequences

**Introduced:** Preview 5.

`FullJoin` returns matches and unmatched elements from both sequences, with
operators for synchronous, queryable, and asynchronous LINQ.

```csharp
var rows = products.FullJoin(
    sales, p => p.Sku, s => s.Sku,
    (product, sale) => new { product, sale });
```

[Source: .NET 11 Preview 5 libraries](https://github.com/dotnet/core/blob/main/release-notes/11.0/preview/preview5/libraries.md#linq-adds-full-outer-joins)

### Validate data asynchronously

**Introduced:** Preview 6.

DataAnnotations adds asynchronous validation attributes, model interfaces, and
`Validator` methods. Use these when a rule needs I/O, such as checking a
database or remote registry, instead of blocking a thread.

```csharp
var context = new ValidationContext(model, services, items: null);
await Validator.ValidateObjectAsync(
    model, context, validateAllProperties: true);
```

[Source: .NET 11 Preview 6 libraries](https://github.com/dotnet/core/blob/main/release-notes/11.0/preview/preview6/libraries.md#asynchronous-validation-with-dataannotations)

### Use newer cryptography and faster `Half` operations

**Introduced:** Preview 5 (X25519); RC 1 (AES Key Wrap and hardware-accelerated `Half` arithmetic).

The platform adds X25519 key agreement and AES Key Wrap APIs. The JIT also
uses hardware FP16 instructions for `Half` arithmetic and conversions when
available.

```csharp
using X25519DiffieHellman key = X25519DiffieHellman.GenerateKey();
Half result = (Half)1.5f + (Half)2.0f;
```

[Source: .NET 11 Preview 5 X25519](https://github.com/dotnet/core/blob/main/release-notes/11.0/preview/preview5/libraries.md#cryptography-adds-x25519-key-agreement) · [RC 1 AES Key Wrap](https://github.com/dotnet/core/blob/main/release-notes/11.0/preview/rc1/libraries.md#aes-key-wrap-support) · [RC 1 runtime](https://github.com/dotnet/core/blob/main/release-notes/11.0/preview/rc1/runtime.md#half-operations-use-fp16-hardware-instructions)

### Runtime-managed async (preview)

**Introduced:** Preview 1; continued to evolve through RC 1.

Runtime Async V2 moves async suspension/resumption state management from
compiler-generated state machines into the runtime. It aims to improve live
stack traces, debugging, and async overhead. It remains opt-in preview
functionality.

```xml
<PropertyGroup>
  <Features>runtime-async=on</Features>
</PropertyGroup>
```

[Source: .NET 11 runtime documentation](https://learn.microsoft.com/en-us/dotnet/core/whats-new/dotnet-11/runtime#runtime-async)

## SDK and build

### Run the CLI and file-based apps more efficiently

**Introduced:** Preview 1 (file-based apps); Preview 4 (Native AOT CLI foundation); Preview 5 (opt-in AOT fast path); Preview 6 (full command surface); Preview 7 (enabled by default). File-based app improvements continued in Previews 3–7 and RC 1.

The `dotnet` CLI uses a Native AOT entry point for common commands and falls
back to the managed CLI when needed. File-based apps gain include directives,
DLL references, formatting support, and cached Native AOT outputs.

```csharp
#:include helpers.cs
#:include ./libs/MyLibrary.dll

Console.WriteLine(Helpers.FormatOutput());
```

[Source: .NET 11 SDK documentation](https://learn.microsoft.com/en-us/dotnet/core/whats-new/dotnet-11/sdk#cli-workflow-and-developer-productivity)

### Pass environment variables and use improved watch workflows

**Introduced:** Preview 1 (interactive `dotnet run` device selection and Hot Reload reference changes); Preview 3 (`dotnet run -e` and watch crash recovery); Preview 4 (device selection in `dotnet watch`).

`dotnet run -e` sets app environment values for one launch. `dotnet watch`
improves crash recovery, device selection for mobile apps, and hot reload
integration.

```console
dotnet run -e ASPNETCORE_ENVIRONMENT=Development -e LOG_LEVEL=Debug
dotnet watch --device <device-id>
```

[Source: .NET 11 SDK documentation](https://learn.microsoft.com/en-us/dotnet/core/whats-new/dotnet-11/sdk#pass-environment-variables-with-dotnet-run)

### Improve test runs

**Introduced:** Preview 1 (positional test targets); Previews 6–7 (test-run options and controls); RC 1 (mobile test projects and device testing).

Microsoft.Testing.Platform integration adds mobile test targets, run-wide
timeouts and failure limits, separate result directories, and support for
traversal projects.

```console
dotnet test --timeout 90s --maximum-failed-tests 5
dotnet test --results-directory-layout per-module
```

[Source: .NET 11 RC 1 SDK](https://github.com/dotnet/core/blob/main/release-notes/11.0/preview/rc1/sdk.md#dotnet-test-adds-run-level-controls-and-result-layouts)

### Publish reproducible containers

**Introduced:** RC 1.

Set `SOURCE_DATE_EPOCH` to make repeated container publishes from the same
inputs produce the same image digest. Registry publishing also skips redundant
uploads when the image manifest already exists.

```console
dotnet publish /t:PublishContainer -p:SOURCE_DATE_EPOCH=1791892800
```

[Source: .NET 11 RC 1 SDK](https://github.com/dotnet/core/blob/main/release-notes/11.0/preview/rc1/sdk.md#container-publishing-produces-reproducible-images-and-skips-redundant-uploads)

### Add tar support and expose MSBuild globs

**Introduced:** RC 1.

MSBuild includes `TarDirectory` and `Untar` tasks. Build tools can also request
unevaluated include, exclude, and remove glob patterns as items.

```xml
<Target Name="ArchiveOutput" AfterTargets="Publish">
  <TarDirectory SourceDirectory="$(PublishDir)"
                DestinationFile="$(OutputPath)package.tar.gz"
                Compression="GZip" />
</Target>
```

[Source: .NET 11 RC 1 MSBuild](https://github.com/dotnet/core/blob/main/release-notes/11.0/preview/rc1/msbuild.md#create-and-extract-tar-archives)

## ASP.NET Core and Blazor

### Protect endpoints from cross-origin requests by default

**Introduced:** Preview 6.

ASP.NET Core apps created with `WebApplication.CreateBuilder` automatically
reject unsafe browser cross-origin requests based on fetch metadata and origin
headers. No endpoint setup is needed for the default protection.

```csharp
var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();
app.MapPost("/transfer", (Transfer request) => Results.Ok());
```

[Source: .NET 11 Preview 6 ASP.NET Core](https://github.com/dotnet/core/blob/main/release-notes/11.0/preview/preview6/aspnetcore.md#automatic-cross-origin-csrf-protection)

### Use async validation in Minimal APIs

**Introduced:** Preview 6.

Minimal API request validation runs asynchronous DataAnnotations validators,
so endpoint validation can perform I/O without blocking threads.

```csharp
builder.Services.AddValidation();
app.MapPost("/reservations", (ReservationRequest request) =>
    Results.Ok(request));
```

[Source: .NET 11 Preview 6 ASP.NET Core](https://github.com/dotnet/core/blob/main/release-notes/11.0/preview/preview6/aspnetcore.md#async-validation-for-minimal-apis)

### Short-circuit selected endpoints

**Introduced:** Preview 6.

`[ShortCircuit]` runs an endpoint immediately after routing, skipping the
remaining middleware pipeline. This is useful for simple endpoints such as
health checks that don't need authentication or other middleware.

```csharp
app.MapGet("/health", [ShortCircuit] () => "Healthy");
```

[Source: .NET 11 Preview 6 ASP.NET Core](https://github.com/dotnet/core/blob/main/release-notes/11.0/preview/preview6/aspnetcore.md#short-circuit-endpoints-with-an-attribute)

### Describe server-sent events in OpenAPI

**Introduced:** Preview 7.

OpenAPI 3.2 documents for endpoints returning `SseItem<T>` use `itemSchema` to
describe each event's payload instead of describing the stream as a string.

```csharp
app.MapGet("/todos/stream", (CancellationToken ct) =>
    TypedResults.ServerSentEvents(GetTodosAsync(ct)));
```

[Source: .NET 11 Preview 7 ASP.NET Core](https://github.com/dotnet/core/blob/main/release-notes/11.0/preview/preview7/aspnetcore.md#openapi-server-sent-events-in-openapi-32)

### Navigate relatively and carry temporary data in Blazor

**Introduced:** Preview 1 (relative navigation); Preview 2 (TempData for Blazor).

Blazor navigation can resolve a path relative to the current URI. Static SSR
also supports TempData for one-time messages across requests and redirects.

```csharp
Navigation.NavigateTo("/configuration",
    new NavigationOptions { RelativeToCurrentUri = true });
```

```csharp
[SupplyParameterFromTempData]
public string? Message { get; set; }
```

[Source: ASP.NET Core 11 documentation](https://learn.microsoft.com/en-us/aspnet/core/release-notes/aspnetcore-11?view=aspnetcore-11.0#navigate-to-and-navlink-support-for-relative-navigation)

### Pause inactive Blazor Server circuits

**Introduced:** Preview 6 (server-triggered pause API); Preview 7 (automatic pause on hidden tabs).

An opt-in AutoPause package can pause a circuit after a browser tab has been
hidden for a configured delay, freeing server resources until the user returns.

```csharp
app.MapRazorComponents<App>()
    .WithBrowserOptions(options =>
        options.AddAutoPause(p => p.HiddenDelay = TimeSpan.FromSeconds(30)));
```

[Source: .NET 11 Preview 7 ASP.NET Core](https://github.com/dotnet/core/blob/main/release-notes/11.0/preview/preview7/aspnetcore.md#auto-pause-blazor-circuits-on-inactivity)

### Cache server-rendered component subtrees

**Introduced:** Preview 7.

`CacheView` replays cached markup without instantiating or rendering the child
components on a cache hit. Vary-by settings help keep output appropriate to
the request.

```razor
<CacheView ExpiresAfter="TimeSpan.FromMinutes(5)"
           VaryByQuery="category">
    <ProductList Category="@Category" />
</CacheView>
```

[Source: .NET 11 Preview 7 ASP.NET Core](https://github.com/dotnet/core/blob/main/release-notes/11.0/preview/preview7/aspnetcore.md#cache-blazor-ssr-output-with-cacheview)

### Improve QuickGrid interaction

**Introduced:** Preview 1 (`OnRowClick`); Preview 5 (URL-based sorting and pagination); Preview 7 (virtualized-grid scroll APIs).

QuickGrid supports row-click callbacks and URL-based paging/sorting, allowing
users to share grid state and use browser history. Virtualized grids can also
scroll directly to a row.

```razor
<QuickGrid Items="@people.AsQueryable()"
           OnRowClick="@(person => SelectPerson(person))">
    <PropertyColumn Property="@(p => p.Name)" />
</QuickGrid>
```

[Source: ASP.NET Core 11 QuickGrid documentation](https://learn.microsoft.com/en-us/aspnet/core/release-notes/aspnetcore-11?view=aspnetcore-11.0#quickgrid-improvements)

### Add environment and metadata-aware UI

**Introduced:** Preview 1; the environment component was renamed from `EnvironmentBoundary` to `EnvironmentView` in Preview 6.

Blazor adds `EnvironmentView` for environment-specific content and `DisplayName`
for labels based on `[Display]`/`[DisplayName]` metadata.

```razor
<EnvironmentView Include="Development">
    <p>Debug mode enabled</p>
</EnvironmentView>
<DisplayName For="@(() => Model!.ProductionDate)" />
```

[Source: ASP.NET Core 11 Blazor documentation](https://learn.microsoft.com/en-us/aspnet/core/release-notes/aspnetcore-11?view=aspnetcore-11.0#new-displayname-component-and-support-for-display-and-displayname-attributes)

## Entity Framework Core 11

### Use complex types with more mappings

**Introduced:** Preview 1 (complex types with TPT/TPC and JSON); Preview 6 (keys and indexes through complex properties); Preview 7 (additional provider support and refinements).

Complex types work with TPT/TPC inheritance, JSON columns, Cosmos DB, and
scalar keys and indexes. Member-access paths also simplify nested property
configuration.

```csharp
modelBuilder.Entity<Customer>()
    .HasIndex(c => c.Address.PostalCode);
```

[Source: EF Core 11 what's new](https://learn.microsoft.com/en-us/ef/core/what-is-new/ef-core-11.0/whatsnew#complex-types)

### Translate more LINQ and generate leaner SQL

**Introduced:** Preview 2 (`MaxBy`/`MinBy`); Preview 3 (join pruning); Preview 5 (cleaner SQL); Preview 6 (`FullJoin`); Preview 7 (`GroupBy` enhancements).

EF Core adds relational `FullJoin` translation, expands `GroupBy`, and
translates `MaxBy`/`MinBy`. It also prunes redundant joins, ordering keys, and
no-op casts.

```csharp
var results = await context.Customers.FullJoin(
    context.Orders, c => c.Id, o => o.CustomerId,
    (customer, order) => new { customer, order });
```

[Source: EF Core 11 what's new](https://learn.microsoft.com/en-us/ef/core/what-is-new/ef-core-11.0/whatsnew#linq-and-sql-translation)

### Query JSON and create JSON indexes

**Introduced:** Preview 2 (`JSON_CONTAINS`); Preview 3 (`JsonContains`/`JsonPathExists` APIs); Preview 6 (SQL Server JSON indexes).

SQL Server support includes JSON path checks, `JSON_CONTAINS` translation for
primitive collections, and migration-managed indexes over JSON paths.

```csharp
var blogs = await context.Blogs
    .Where(b => EF.Functions.JsonPathExists(b.JsonData, "$.Rating"))
    .ToListAsync();
```

[Source: EF Core 11 what's new](https://learn.microsoft.com/en-us/ef/core/what-is-new/ef-core-11.0/whatsnew#ef-functions-jsonpathexists)

### Search SQL Server with vectors and full text

**Introduced:** Preview 2 (vector indexes/search and full-text support); Preview 4 (approximate vector search).

EF Core can configure vector and full-text indexes and perform approximate
vector or ranked full-text search. Combining both enables hybrid search.

```csharp
var nearest = await context.Blogs
    .VectorSearch(b => b.Embedding, queryVector, "cosine")
    .OrderBy(result => result.Distance)
    .Take(5)
    .WithApproximate()
    .ToListAsync();
```

[Source: EF Core 11 what's new](https://learn.microsoft.com/en-us/ef/core/what-is-new/ef-core-11.0/whatsnew#sql-server)

### Simplify migration workflows

**Introduced:** Preview 1.

The EF CLI can create, compile, and apply a migration in one invocation, which
is useful where rebuilding the application isn't practical.

```console
dotnet ef database update InitialCreate --add
```

[Source: EF Core 11 what's new](https://learn.microsoft.com/en-us/ef/core/what-is-new/ef-core-11.0/whatsnew#create-and-apply-migrations-in-one-step)

### Inspect tracked entries without extra change detection

**Introduced:** Preview 3.

`ChangeTracker.GetEntriesForState()` returns tracked entities in the selected
states without first forcing a `DetectChanges()` pass. This avoids redundant
work in long-lived contexts and high-volume change-tracking code.

```csharp
var modified = context.ChangeTracker.GetEntriesForState(
    added: false,
    modified: true,
    deleted: false,
    unchanged: false);
```

[Source: .NET 11 Preview 3 EF Core](https://github.com/dotnet/core/blob/main/release-notes/11.0/preview/preview3/efcore.md#changetrackergetentriesforstate-avoids-extra-change-detection)

### Warn when async queries run synchronously

**Introduced:** Preview 5.

The EF Core analyzer reports EF1004 when `ToAsyncEnumerable()` is called on an
`IQueryable<T>`, because that wraps synchronous enumeration. Use
`AsAsyncEnumerable()` so the query runs through EF Core's async pipeline.

```csharp
// EF1004: this can evaluate the EF query synchronously.
await foreach (var blog in blogsQuery.ToAsyncEnumerable()) { }

// Use EF Core's async query pipeline instead.
await foreach (var blog in blogsQuery.AsAsyncEnumerable()) { }
```

[Source: .NET 11 Preview 5 EF Core](https://github.com/dotnet/core/blob/main/release-notes/11.0/preview/preview5/efcore.md#ef1004-warns-when-async-ef-queries-run-synchronously)

## Windows Forms

### Opt into modern visual styles and avoid startup flashes

**Introduced:** Preview 7.

Windows Forms can opt into the .NET 11 rendering mode and defer revealing a
form until its initial layout and theming are ready.

```csharp
Application.SetDefaultVisualStylesMode(VisualStylesMode.Net11);
Application.SetDefaultFormRevealMode(FormRevealMode.Deferred);
```

[Source: .NET 11 Preview 7 Windows Forms](https://github.com/dotnet/core/blob/main/release-notes/11.0/preview/preview7/winforms.md#opt-in-to-net-11-visual-styles)

### Render check boxes as toggle switches

**Introduced:** Preview 7.

`Appearance.ToggleSwitch` renders a `CheckBox` as a switch under
`VisualStylesMode.Net11`. `Checked`, `CheckedChanged`, and data binding behave
as before, so adopting it doesn't require code changes.

```csharp
CheckBox toggle = new()
{
    Text = "Enable notifications",
    Appearance = Appearance.ToggleSwitch,
    Checked = true,
};
```

[Source: .NET 11 Preview 7 Windows Forms](https://github.com/dotnet/core/blob/main/release-notes/11.0/preview/preview7/winforms.md#toggle-switch-appearance-for-checkbox-radiobutton)

### Suspend painting during bulk updates

**Introduced:** Preview 7.

`SuspendPainting` scopes layout and painting suspension to reduce flicker
while many control items are updated.

```csharp
using (list.SuspendPainting(LayoutSuspendTraversal.Target))
{
    for (int i = 0; i < 10_000; i++)
        list.Items.Add($"Item {i}");
}
```

[Source: .NET 11 Preview 7 Windows Forms](https://github.com/dotnet/core/blob/main/release-notes/11.0/preview/preview7/winforms.md#suspend-painting-during-bulk-mutations)

## WPF

### Reliability and compatibility fixes

**Introduced:** Preview 1; additional fixes arrived in Previews 2, 3, and 6, with native-code hardening in RC 1.

WPF did not add user-facing features in the reported .NET 11 previews, but
release notes do record several reliability and compatibility fixes:

- Preview 1 fixes Fluent window backdrop and background behavior on Windows 10.
- Preview 2 guards against crashes caused by Desktop Window Manager failures.
- Preview 3 avoids an `InvalidOperationException` during weak-event table
  cleanup when `EnableWeakEventMemoryImprovements` is enabled.
- Preview 6 includes `ijwhost.dll` when publishing self-contained WPF apps,
  preventing launch failures with mixed-mode WPF assemblies.
- RC 1 hardens text, graphics, fonts, printing, XPS, and resource loading.

[Sources: .NET 11 Preview 1 WPF](https://github.com/dotnet/core/blob/main/release-notes/11.0/preview/preview1/wpf.md), [Preview 2 WPF](https://github.com/dotnet/core/blob/main/release-notes/11.0/preview/preview2/wpf.md), [Preview 3 WPF](https://github.com/dotnet/core/blob/main/release-notes/11.0/preview/preview3/wpf.md), [Preview 6 WPF](https://github.com/dotnet/core/blob/main/release-notes/11.0/preview/preview6/wpf.md), and [RC 1 WPF](https://github.com/dotnet/core/blob/main/release-notes/11.0/preview/rc1/wpf.md).

## Related sources

The full preview-by-preview release notes and the EF Core 11 and C# 15
documentation are indexed in [`sources.txt`](sources.txt). For SDK-specific
details—including APIs that changed names between previews—consult those
original pages and the comments in the corresponding sample projects.
