// C# 15 extends the "extension members" feature (introduced in C# 14, see dotnet10/CSharp14.ExtensionMembers)
// with EXTENSION INDEXERS: `this[...]` can now be declared inside an `extension(Type instance) { ... }`
// block, adding real indexer syntax (`obj[key]`, `obj[key] = value`) to a type you don't own.
// This is still in Preview as of .NET 11 Preview 6, so it requires <LangVersion>preview</LangVersion>.

// Arrays and Spans support Range indexing out of the box (`array[1..3]`), but List<T> does not -
// GetRange(index, count) is the closest it offers. An extension indexer closes that gap cleanly:
var numbers = new List<int> { 10, 20, 30, 40, 50 };
Console.WriteLine(string.Join(", ", numbers[1..3])); // 20, 30

// Extension indexers can take multiple parameters and support both get and set, just like a
// regular indexer - here we add a two-key indexer to Dictionary<string, int>:
var scores = new Dictionary<string, int> { ["alice"] = 10, ["bob"] = 20 };
Console.WriteLine(scores["alice", "bob"]); // 30 (sum of both)

scores["alice", "bob"] = 5; // set accessor: assigns 5 to BOTH keys
Console.WriteLine(string.Join(", ", scores.Select(kv => $"{kv.Key}={kv.Value}")));

public static class Extensions
{
    extension<T>(List<T> source)
    {
        public List<T> this[Range range]
        {
            get
            {
                var (offset, length) = range.GetOffsetAndLength(source.Count);
                return source.GetRange(offset, length);
            }
        }
    }

    extension(Dictionary<string, int> source)
    {
        public int this[string keyA, string keyB]
        {
            get => source[keyA] + source[keyB];
            set { source[keyA] = value; source[keyB] = value; }
        }
    }
}
