// C# 15 (.NET 11 Preview 7) adds LABELED break/continue: you can put a label before a loop and
// then `break <label>;` or `continue <label>;` from inside a NESTED loop to target that specific
// outer loop, without restructuring the code into a helper method (for an early `return`) or
// threading a boolean "found" flag through every level.
//
// Still Preview as of .NET 11 Preview 6/7, hence <LangVersion>preview</LangVersion>.

int[,] grid =
{
    { 1, 2, 3 },
    { 4, 5, 6 },
    { 7, 8, 9 },
};

// --- break <label>: stop the OUTER loop entirely from inside the inner one ---
searchGrid:
for (int row = 0; row < grid.GetLength(0); row++)
{
    for (int col = 0; col < grid.GetLength(1); col++)
    {
        Console.WriteLine($"checking [{row},{col}] = {grid[row, col]}");
        if (grid[row, col] == 5)
        {
            Console.WriteLine($"found 5 at [{row},{col}], stopping the outer loop");
            break searchGrid;
        }
    }
}
// Before this feature, the same result needed either a `found` flag checked in both loops,
// or a `goto`, or extracting the search into a separate method just so `return` could exit both loops at once.

Console.WriteLine();

// --- continue <label>: skip to the NEXT ITERATION of the OUTER loop from inside the inner one ---
rows:
for (int row = 0; row < grid.GetLength(0); row++)
{
    for (int col = 0; col < grid.GetLength(1); col++)
    {
        if (grid[row, col] % 4 == 0)
        {
            Console.WriteLine($"row {row} contains a multiple of 4 ({grid[row, col]}) - skipping the rest of this row");
            continue rows;
        }
    }

    Console.WriteLine($"row {row} has no multiples of 4");
}
