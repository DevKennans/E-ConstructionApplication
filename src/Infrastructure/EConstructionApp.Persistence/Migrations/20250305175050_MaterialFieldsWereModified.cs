using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EConstructionApp.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class MaterialFieldsWereModified : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddCheckConstraint(
                name: "CK_Material_Name_Length",
                table: "Materials",
                sql: "LEN(Name) >= 2");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Material_Price",
                table: "Materials",
                sql: "Price > 0");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Material_StockQuantity",
                table: "Materials",
                sql: "StockQuantity >= 0");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_Material_Name_Length",
                table: "Materials");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Material_Price",
                table: "Materials");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Material_StockQuantity",
                table: "Materials");
        }
    }
}
