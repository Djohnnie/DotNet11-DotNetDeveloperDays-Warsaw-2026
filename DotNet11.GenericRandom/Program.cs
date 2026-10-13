// .NET 11 adds generic numeric APIs to System.Random.
// Previously, generating a random value for a numeric type required branching per type:
// Next() for int, NextInt64() for long, NextSingle() for float, NextDouble() for double, and so on.
// The new generic overloads let generic algorithms request a random value for any numeric type parameter
// without that branching:
// - NextInteger<T>() / NextInteger<T>(maxValue) / NextInteger<T>(minValue, maxValue) for integer types
// - NextBinaryFloat<T>() for floating-point types, returning a value in the range [0.0, 1.0)

Random random = Random.Shared;

var diceRoll = random.NextInteger<int>(1, 7);
Console.WriteLine($"Dice roll (1-6): {diceRoll}");

var id = random.NextInteger<long>();
Console.WriteLine($"Random Int64 id: {id}");

var sample = random.NextBinaryFloat<Half>();
Console.WriteLine($"Random Half in [0.0, 1.0): {sample}");

// The same generic method works unchanged for any numeric type parameter T.
PrintRandomInteger<byte>(random);
PrintRandomInteger<short>(random);
PrintRandomInteger<uint>(random);

static void PrintRandomInteger<T>(Random random) where T : System.Numerics.IBinaryInteger<T>, System.Numerics.IMinMaxValue<T>
{
    var value = random.NextInteger<T>();
    Console.WriteLine($"Random {typeof(T).Name}: {value}");
}

var randomNumber = random.Next();

var randomInt = random.NextInteger<int>();

var randomLong = random.NextInteger<long>();

var randomFloat = random.NextBinaryFloat<float>();

var randomDouble = random.NextBinaryFloat<double>();


