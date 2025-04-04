using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EConstructionApp.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class FiledOfMaterialTransactionLogWasUpdated : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ModifiedDate",
                table: "MaterialTransactionLogs");

            migrationBuilder.AddColumn<DateTime>(
                name: "InsertedDate",
                table: "MaterialTransactionLogs",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "18FF4584-E530-4CFE-ADC1-9485EBCC1982",
                column: "ConcurrencyStamp",
                value: "1ba72fd7-0796-4f95-8234-2e9ed68e0e18");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "A6975C7C-5397-4BBB-B450-2790781BCBAB",
                column: "ConcurrencyStamp",
                value: "f6327ec6-f6b1-4bd0-a7e9-13759eeda6aa");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "BA54FA79-8361-4560-B1A5-C55097FE6E63",
                column: "ConcurrencyStamp",
                value: "1701d603-54a6-494f-9e34-5d9f96ddd34c");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "43B64316-DE56-4300-AE86-C298AEA73C7B",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "f767c20b-a767-4f92-bb30-fa999b0b6118", "AQAAAAIAAYagAAAAEM+Yc255WP32/VMAtG9CmWtXvvfCdriKpKqa5XVpzoua5yML7h92nOpi0BtGPvWj8Q==", "575262d7-9c98-4a9c-af39-02c8808b4ba8" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "99D74F43-7E23-41BE-9F98-10C5D6130312",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "a7697875-c857-40f0-a81d-cce8c3654606", "AQAAAAIAAYagAAAAEMDtR/+1f3/femIvUlaEQNGQwWwzjPctEEPx/X5glPl0yxEMl8lsnATyzd31h3r9Pw==", "c1d6ea81-d7d8-4dac-8775-73629db0dffc" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "InsertedDate",
                table: "MaterialTransactionLogs");

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedDate",
                table: "MaterialTransactionLogs",
                type: "datetime2",
                nullable: true);

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
    }
}
