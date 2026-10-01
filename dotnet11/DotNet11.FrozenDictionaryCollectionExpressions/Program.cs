using System.Collections.Frozen;

// FrozenDictionary<TKey,TValue> and FrozenSet<T> optimize themselves at construction time for
// very fast read-only lookups, at the cost of a more expensive build step - a great fit for
// data that's built once (e.g. at startup) and read many times afterwards.
// Before .NET 11 you always had to build them from an existing collection via .ToFrozenDictionary().
// .NET 11 adds a FrozenDictionary.Create(ReadOnlySpan<KeyValuePair<TKey,TValue>>) factory that the
// compiler recognizes as a collection-expression builder, so you can now write a FrozenDictionary
// directly with collection expression syntax - no intermediate Dictionary required.

FrozenDictionary<string, int> populationByCountry =
[
    KeyValuePair.Create("Belgium", 11_700_000),
    KeyValuePair.Create("Poland", 37_700_000),
    KeyValuePair.Create("Netherlands", 17_800_000),
];

foreach (var (country, population) in populationByCountry)
{
    Console.WriteLine($"{country,-12} {population,12:N0}");
}

Console.WriteLine($"Runtime type: {populationByCountry.GetType().Name}");
Console.WriteLine($"Belgium found: {populationByCountry.TryGetValue("Belgium", out var value)}, population: {value:N0}");

// The old way still works and produces the exact same result - collection expressions are just
// a more concise way to reach the same Create(...) factory.
FrozenDictionary<string, int> same = new Dictionary<string, int>
{
    ["Belgium"] = 11_700_000,
    ["Poland"] = 37_700_000,
    ["Netherlands"] = 17_800_000,
}.ToFrozenDictionary();

Console.WriteLine($"Both approaches agree: {populationByCountry.OrderBy(kv => kv.Key).SequenceEqual(same.OrderBy(kv => kv.Key))}");
