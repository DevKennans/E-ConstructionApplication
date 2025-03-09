using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EConstructionApp.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class DefaultTokenStructureWasDone : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RefreshToken",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "RefreshTokenEndDate",
                table: "AspNetUsers");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "18FF4584-E530-4CFE-ADC1-9485EBCC1982",
                column: "ConcurrencyStamp",
                value: "a005bbe4-a447-4fd7-8702-81aed5f3741c");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "A6975C7C-5397-4BBB-B450-2790781BCBAB",
                column: "ConcurrencyStamp",
                value: "10ebcf35-e410-45f2-9e18-eae276d4f642");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "BA54FA79-8361-4560-B1A5-C55097FE6E63",
                column: "ConcurrencyStamp",
                value: "4b94c496-a38b-4539-ae33-e5198a2bd6af");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "43B64316-DE56-4300-AE86-C298AEA73C7B",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "6b3dc824-665b-4b59-a81d-c71119c407eb", "AQAAAAIAAYagAAAAENwK6WhaDMD4uJ6OD1mXzrCl590nwoXQSk0dw8AwG8mokHIFclW2h62O+lb/f6rJkg==", "0d3e002e-942f-4412-82b8-ca269bbd0056" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "99D74F43-7E23-41BE-9F98-10C5D6130312",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "20cc43b2-20cd-43fa-80c7-1c206279d857", "AQAAAAIAAYagAAAAECKEYFNZNBYsKIP0ShA4HA28blHf1EMwTf4Rd8hNCS2UVR73nFObUdqiOijjebsHpQ==", "1e7d7196-0c23-474b-a51e-ac54d8f3e86a" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "RefreshToken",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "RefreshTokenEndDate",
                table: "AspNetUsers",
                type: "datetime2",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "18FF4584-E530-4CFE-ADC1-9485EBCC1982",
                column: "ConcurrencyStamp",
                value: "43693f47-56e3-4833-9f7b-0cef9e5ef63c");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "A6975C7C-5397-4BBB-B450-2790781BCBAB",
                column: "ConcurrencyStamp",
                value: "5593d5fe-45fb-4f4a-924d-e20af6868b6f");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "BA54FA79-8361-4560-B1A5-C55097FE6E63",
                column: "ConcurrencyStamp",
                value: "1ca9a86d-d352-4394-9331-c29ac21a362b");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "43B64316-DE56-4300-AE86-C298AEA73C7B",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "RefreshToken", "RefreshTokenEndDate", "SecurityStamp" },
                values: new object[] { "ce0ab655-0226-4ac2-943c-431e29f7af9b", "AQAAAAIAAYagAAAAEJYSL7vxjZzYOwHin6MiEF6uNmvfFjktqTPWln0YTKbnzBNBpg79OX6RS+AjJP2NKg==", null, null, "cd38bcfd-6f0b-43c9-bd9f-2599b7cb992b" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "99D74F43-7E23-41BE-9F98-10C5D6130312",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "RefreshToken", "RefreshTokenEndDate", "SecurityStamp" },
                values: new object[] { "9ab13702-d640-4eef-9976-eac2e2fdcd97", "AQAAAAIAAYagAAAAEAj9ZiT68NtaZa1bGWb8LlXACX91WWmXcHor1wzo+/Q6Eo34flL4nXQZoEe9SwKPPg==", null, null, "44cb9d76-203d-4816-af50-c90b125e79b5" });
        }
    }
}
