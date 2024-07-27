using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DecorStore.Data.Migrations
{
    /// <inheritdoc />
    public partial class FixedCategorySchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "Product.Category");

            migrationBuilder.RenameTable(
                name: "Subcategories_tb",
                schema: "Product",
                newName: "Subcategories_tb",
                newSchema: "Product.Category");

            migrationBuilder.RenameTable(
                name: "Sections_tb",
                schema: "Product",
                newName: "Sections_tb",
                newSchema: "Product.Category");

            migrationBuilder.RenameTable(
                name: "Categories_tb",
                schema: "Product",
                newName: "Categories_tb",
                newSchema: "Product.Category");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "Product");

            migrationBuilder.RenameTable(
                name: "Subcategories_tb",
                schema: "Product.Category",
                newName: "Subcategories_tb",
                newSchema: "Product");

            migrationBuilder.RenameTable(
                name: "Sections_tb",
                schema: "Product.Category",
                newName: "Sections_tb",
                newSchema: "Product");

            migrationBuilder.RenameTable(
                name: "Categories_tb",
                schema: "Product.Category",
                newName: "Categories_tb",
                newSchema: "Product");
        }
    }
}
