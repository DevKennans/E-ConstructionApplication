using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EConstructionApp.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class CategoryFieldsWereModified : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddCheckConstraint(
                name: "CK_Category_Name_Length",
                table: "Categories",
                sql: "LEN(Name) >= 2");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_Category_Name_Length",
                table: "Categories");
        }
    }
}
