using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EConstructionApp.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class MaterialOperationsForTaskConfigurations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MaterialTransactionLogs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TaskId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MaterialId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Quantity = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Measure = table.Column<int>(type: "int", nullable: false),
                    PriceAtTransaction = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TransactionType = table.Column<int>(type: "int", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaterialTransactionLogs", x => x.Id);
                    table.CheckConstraint("CK_MaterialTransactionLog_Price", "PriceAtTransaction >= 0");
                    table.CheckConstraint("CK_MaterialTransactionLog_Quantity", "Quantity > 0");
                });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "18FF4584-E530-4CFE-ADC1-9485EBCC1982",
                column: "ConcurrencyStamp",
                value: "fde57bf6-c3a9-4615-b9f1-5bcd7d0666d8");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "A6975C7C-5397-4BBB-B450-2790781BCBAB",
                column: "ConcurrencyStamp",
                value: "a860f514-7a48-4b3d-8a2a-469fa1ee49a6");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "BA54FA79-8361-4560-B1A5-C55097FE6E63",
                column: "ConcurrencyStamp",
                value: "d9c3b0e5-fc5f-4e68-b849-79f6dab2f222");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "43B64316-DE56-4300-AE86-C298AEA73C7B",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "fcff519a-cf00-4068-b07d-9ff394a32f71", "AQAAAAIAAYagAAAAEN73P6AiMKO1ErSwI8BM1f+qg8aHt+1Rtxo0c2ciB5+hZa7x0uXGp43/a99Ya9l+gA==", "4b28b429-ae5e-48cc-8ff4-cafd59cc53bf" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "99D74F43-7E23-41BE-9F98-10C5D6130312",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "ce116652-0a7e-4553-a174-0ba8e0d4a058", "AQAAAAIAAYagAAAAEKLVvisjHmq+OIECRPFGYxesD8lWDlpxNF2ko5xskOwtANDlLCOVJWiMnW+G63n9lg==", "d57e5e37-0ea1-425c-ad33-dfc869c98394" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MaterialTransactionLogs");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "18FF4584-E530-4CFE-ADC1-9485EBCC1982",
                column: "ConcurrencyStamp",
                value: "5df60643-ec4e-4636-8fda-ed1f65ab9a94");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "A6975C7C-5397-4BBB-B450-2790781BCBAB",
                column: "ConcurrencyStamp",
                value: "6f1112ca-7771-46b6-9990-bfa36c0b4534");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "BA54FA79-8361-4560-B1A5-C55097FE6E63",
                column: "ConcurrencyStamp",
                value: "b5142338-e893-4e1e-a2a0-23c0a96bda8c");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "43B64316-DE56-4300-AE86-C298AEA73C7B",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "324b86d1-2bf0-4a0b-9c69-8684f5a9a7c4", "AQAAAAIAAYagAAAAEDGIx7NYvgHphwCzPiPpfd1bOD9yijxIuzsMo0dE1Nv3oD8ikubMz1jbEJsBpthUsA==", "8cf91e04-8940-4153-b94b-168b2d71c0e6" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "99D74F43-7E23-41BE-9F98-10C5D6130312",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "d2c8e713-46d3-45cc-bf76-9b8ed2afbf1c", "AQAAAAIAAYagAAAAEOH1oXF3psZQZ3+oIsypwUf0xbmnRrcmmXvhmkta0oGoMqd8CxArUbb+YjyUhvxZ/w==", "c4fa41a5-716b-4bb6-9ef7-d62041e323b8" });
        }
    }
}
