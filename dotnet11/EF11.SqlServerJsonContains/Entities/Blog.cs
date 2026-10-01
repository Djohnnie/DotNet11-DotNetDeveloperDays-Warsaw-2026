namespace EF11.SqlServerJsonContains.Entities;

public class Blog
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;

    // EF Core maps a List<string> of primitives to a JSON array column automatically.
    public List<string> Tags { get; set; } = [];

    public BlogDetails Details { get; set; } = new();
}

// A complex type mapped to a JSON column, so we can also demo EF.Functions.JsonContains()
// against a specific JSON path.
public class BlogDetails
{
    public int Rating { get; set; }
}
