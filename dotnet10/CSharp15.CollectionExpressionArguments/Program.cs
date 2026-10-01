// Starting in C# 15, you can pass arguments to the underlying collection's constructor or factory method
// by using a with(...) element as the first element in a collection expression.
// This feature enables you to specify capacity, comparers, or other constructor parameters
// directly within the collection expression syntax.
// The with(...) element must be the first element in the collection expression.
// The arguments declared in the with(...) element are passed to the appropriate constructor or create method based on the target type.
// You can use any valid expression for the arguments in the with element.


using System.Runtime.CompilerServices;

string[] values = ["one", "two", "three"];
PrintAll(values);

// Pass capacity argument to List<T> constructor
List<string> names = [with(capacity: values.Length * 2), .. values];
PrintAll(names);

// Pass comparer argument to HashSet<T> constructor
// Set contains only one element because all strings are equal with OrdinalIgnoreCase
HashSet<string> set = [with(StringComparer.OrdinalIgnoreCase), "Hello", "HELLO", "hello"];
PrintAll(set);

MyRandomIntList ints1 = [with(8), 0, 0, 0, 0, 0, 0, 0, 0];
PrintAll(ints1);

MyRandomIntList ints2 = [with([0, 1], numberOfItems: 8), 0, 0, 0, 0, 0, 0, 0, 0];
PrintAll(ints2);

MyRandomSrtringList strings1 = [with(8), "X", "X", "X", "X", "X", "X", "X", "X"];
PrintAll(strings1);

MyRandomSrtringList strings2 = [with(["A", "T", "G", "C"], 8), "X", "X", "X", "X", "X", "X", "X", "X"];
PrintAll(strings2);

Console.ReadKey();


static void PrintAll<T>(IEnumerable<T> items)
{
    Console.WriteLine(string.Join(", ", items));
}


class MyRandomIntList : List<int>
{
    public MyRandomIntList(int numberOfItems)
    {
        this.AddRange(Random.Shared.GetItems<int>([0, 1], numberOfItems));
    }

    public MyRandomIntList(int[] choices, int numberOfItems)
    {
        this.AddRange(Random.Shared.GetItems<int>(choices, numberOfItems));
    }
}

[CollectionBuilder(typeof(MyRandomSrtringListBuilder), nameof(MyRandomSrtringListBuilder.Create))]
class MyRandomSrtringList : List<string>
{

}

class MyRandomSrtringListBuilder
{
    public static MyRandomSrtringList Create(int numberOfItems, ReadOnlySpan<string> items)
    {
        return Create(["X", "O"], numberOfItems, items);
    }

    public static MyRandomSrtringList Create(string[] choices, int numberOfItems, ReadOnlySpan<string> items)
    {
        var list = new MyRandomSrtringList();
        list.AddRange(Random.Shared.GetItems<string>(choices, numberOfItems));
        list.AddRange(items);
        return list;
    }
}