// C# 15 collection expression ARGUMENTS: a `with(...)` element, as the first element of a collection
// expression, forwards arguments to the underlying collection's constructor or factory method - for
// example to pre-size a List<T> or supply a comparer to a HashSet<T>/Dictionary<TKey,TValue>.
// This was already previewed under the .NET 10 SDK (see dotnet10/CSharp15.CollectionExpressionArguments) -
// the syntax is unchanged in .NET 11, but it's STILL Preview here too, so <LangVersion>preview</LangVersion>
// is still required.

using System.Runtime.CompilerServices;

// Pre-size a List<T> - avoids the internal array being resized/reallocated as items are added:
List<string> names = [with(capacity: 10), "Ada", "Alan", "Grace"];
Console.WriteLine($"names.Capacity = {names.Capacity} (Count = {names.Count})");

// Supply a comparer to a HashSet<T> - here, case-insensitive, so "HI" and "hi" collapse to one entry:
HashSet<string> greetings = [with(StringComparer.OrdinalIgnoreCase), "Hi", "HI", "hi", "Bye"];
Console.WriteLine(string.Join(", ", greetings));

// Supply BOTH a capacity and a comparer to a HashSet<T>:
HashSet<string> codes = [with(capacity: 100, comparer: StringComparer.OrdinalIgnoreCase), "A1", "B2"];
Console.WriteLine($"codes.Contains(\"a1\") = {codes.Contains("a1")}"); // case-insensitive lookup

// The with(...) element also works with a CUSTOM collection type, as long as it exposes a
// [CollectionBuilder]-annotated factory method whose leading parameters match the with(...) arguments:
RingBuffer<int> ring = [with(capacity: 3), 1, 2, 3, 4, 5]; // capacity 3 -> only the last 3 items are kept
Console.WriteLine(string.Join(", ", ring));

[CollectionBuilder(typeof(RingBufferBuilder), nameof(RingBufferBuilder.Create))]
class RingBuffer<T> : IEnumerable<T>
{
    private readonly Queue<T> _items;
    public RingBuffer(int capacity) => _items = new Queue<T>(capacity);
    public int Capacity { get; init; }

    public void Add(T item, int capacity)
    {
        if (_items.Count == capacity)
        {
            _items.Dequeue();
        }
        _items.Enqueue(item);
    }

    public IEnumerator<T> GetEnumerator() => _items.GetEnumerator();
    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => GetEnumerator();
}

static class RingBufferBuilder
{
    public static RingBuffer<T> Create<T>(int capacity, ReadOnlySpan<T> items)
    {
        var buffer = new RingBuffer<T>(capacity) { Capacity = capacity };
        foreach (var item in items)
        {
            buffer.Add(item, capacity);
        }
        return buffer;
    }
}
