// LINQ has had Join (inner join) and GroupJoin forever, and
// .NET 10 added LeftJoin/RightJoin.
// .NET 11 completes the set with Enumerable.FullJoin: every
// element from BOTH sequences appears
// in the result at least once, with a default value substituted
// on whichever side has no match.

var employees = new[]
{
    new { Name = "Alice", DepartmentId = 1 },
    new { Name = "Bob", DepartmentId = 2 },
    // department that doesn't exist below
    new { Name = "Carol", DepartmentId = 99 },
};

var departments = new[]
{
    new { Id = 1, Department = "Engineering" },
    new { Id = 2, Department = "Sales" },
    // department with no employees below
    new { Id = 3, Department = "Marketing" },
};

var result = employees.FullJoin(
    departments,
    employee => employee.DepartmentId,
    department => department.Id,
    (employee, department) => new
    {
        Employee = employee?.Name ?? "(no employee)",
        Department = department?.Department ?? "(no department)",
    });

foreach (var row in result)
{
    Console.WriteLine($"{row.Employee,-14} | {row.Department}");
}
