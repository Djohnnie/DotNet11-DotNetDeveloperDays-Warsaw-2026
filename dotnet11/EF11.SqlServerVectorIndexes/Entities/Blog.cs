using Microsoft.Data.SqlTypes;

namespace EF11.SqlServerVectorIndexes.Entities;

public class Blog
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public SqlVector<float> Embedding { get; set; }
}
