using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DecorStore.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "Product");

            migrationBuilder.CreateTable(
                name: "Sections_tb",
                schema: "Product",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sections_tb", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Categories_tb",
                schema: "Product",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SectionId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories_tb", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Categories_tb_Sections_tb_SectionId",
                        column: x => x.SectionId,
                        principalSchema: "Product",
                        principalTable: "Sections_tb",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Subcategories_tb",
                schema: "Product",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IconUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CategoryId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Subcategories_tb", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Subcategories_tb_Categories_tb_CategoryId",
                        column: x => x.CategoryId,
                        principalSchema: "Product",
                        principalTable: "Categories_tb",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Categories_tb_SectionId",
                schema: "Product",
                table: "Categories_tb",
                column: "SectionId");

            migrationBuilder.CreateIndex(
                name: "IX_Subcategories_tb_CategoryId",
                schema: "Product",
                table: "Subcategories_tb",
                column: "CategoryId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Subcategories_tb",
                schema: "Product");

            migrationBuilder.DropTable(
                name: "Categories_tb",
                schema: "Product");

            migrationBuilder.DropTable(
                name: "Sections_tb",
                schema: "Product");
        }
    }
}
