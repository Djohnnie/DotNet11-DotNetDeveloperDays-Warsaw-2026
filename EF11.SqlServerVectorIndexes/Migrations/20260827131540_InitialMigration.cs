using Microsoft.Data.SqlTypes;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EF11.SqlServerVectorIndexes.Migrations;

/// <inheritdoc />
public partial class _20260827131540_InitialMigration : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "Blogs",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                Embedding = table.Column<SqlVector<float>>(type: "vector(3)", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Blogs", x => x.Id);
            });

        migrationBuilder.CreateIndex(
            name: "IX_Blogs_Embedding",
            table: "Blogs",
            column: "Embedding")
            .Annotation("SqlServer:VectorIndexMetric", "cosine");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "Blogs");
    }
}
