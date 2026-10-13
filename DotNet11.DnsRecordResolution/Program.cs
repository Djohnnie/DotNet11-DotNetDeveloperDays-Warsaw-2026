// .NET 11 Libraries - Resolve DNS record types.
// Introduced: Preview 7 (Windows implementation); RC 1 (Linux implementation).
//
// System.Net.Dns gains typed record resolution beyond the classic IPHostEntry lookups.
// Dns.ResolveAddresses / ResolveSrv / ResolveMx / ResolveTxt / ResolveCName / ResolvePtr /
// ResolveNs (and their *Async variants) return a DnsResult<T> envelope carrying the records,
// a DnsResponseCode and a NegativeCacheTtl. The DnsResolver type adds instance-based
// resolution driven by DnsResolverOptions, including a custom DNS server list.
//
// Source: https://github.com/dotnet/core/blob/main/release-notes/11.0/preview/preview7/libraries.md#dns-record-resolution-apis

using System.Net;

Console.WriteLine($"Platform support: {RuntimeSupported()}");
Console.WriteLine();

await ShowMxAsync("microsoft.com");
await ShowTxtAsync("microsoft.com");
await ShowSrvAsync("_ldap._tcp.example.com");
await ShowAddressesAsync("dotnet.microsoft.com");

Console.WriteLine("== Instance-based DnsResolver with explicit servers ==");
try
{
    var options = new DnsResolverOptions
    {
        Servers = [new IPEndPoint(IPAddress.Parse("1.1.1.1"), 53)],
    };

    using var resolver = new DnsResolver(options);
    DnsResult<AddressRecord> result = await resolver.ResolveAddressesAsync("example.com");
    Report(result, r => $"{r.Address} (ttl {r.Ttl})");
}
catch (Exception ex)
{
    Console.WriteLine($"  resolver unavailable: {ex.GetType().Name} - {ex.Message}");
}

static string RuntimeSupported() =>
    OperatingSystem.IsWindows() || OperatingSystem.IsLinux()
        ? "DNS record resolution is implemented on this platform."
        : "This platform still throws PlatformNotSupportedException.";

static async Task ShowMxAsync(string name)
{
    Console.WriteLine($"== MX records for {name} ==");
    await SafeAsync(async () =>
    {
        DnsResult<MxRecord> result = await Dns.ResolveMxAsync(name);
        Report(result, r => $"{r.Preference,5} {r.Exchange}");
    });
}

static async Task ShowTxtAsync(string name)
{
    Console.WriteLine($"== TXT records for {name} ==");
    await SafeAsync(async () =>
    {
        DnsResult<TxtRecord> result = await Dns.ResolveTxtAsync(name);
        Report(result, r => string.Join(" | ", r.Values));
    });
}

static async Task ShowSrvAsync(string name)
{
    Console.WriteLine($"== SRV records for {name} ==");
    await SafeAsync(async () =>
    {
        DnsResult<SrvRecord> result = await Dns.ResolveSrvAsync(name);

        // A name with no SRV records is NOT an error: NoError + zero records, plus a
        // NegativeCacheTtl telling you how long the negative answer may be cached.
        Console.WriteLine($"  negative cache ttl: {result.NegativeCacheTtl}");
        Report(result, r => $"{r.Target}:{r.Port} pri={r.Priority} wt={r.Weight}");
    });
}

static async Task ShowAddressesAsync(string name)
{
    Console.WriteLine($"== Address records for {name} ==");
    await SafeAsync(async () =>
    {
        DnsResult<AddressRecord> result = await Dns.ResolveAddressesAsync(name);
        Report(result, r => $"{r.Address} (ttl {r.Ttl})");
    });
}

static void Report<T>(DnsResult<T> result, Func<T, string> format)
{
    Console.WriteLine($"  response code: {result.ResponseCode}");

    if (result.ResponseCode != DnsResponseCode.NoError)
    {
        Console.WriteLine();
        return;
    }

    if (result.Records.Count == 0)
    {
        Console.WriteLine("  (no records)");
    }

    foreach (T record in result.Records)
    {
        Console.WriteLine($"  {format(record)}");
    }

    Console.WriteLine();
}

static async Task SafeAsync(Func<Task> action)
{
    try
    {
        await action();
    }
    catch (Exception ex)
    {
        Console.WriteLine($"  lookup failed: {ex.GetType().Name} - {ex.Message}");
        Console.WriteLine();
    }
}
