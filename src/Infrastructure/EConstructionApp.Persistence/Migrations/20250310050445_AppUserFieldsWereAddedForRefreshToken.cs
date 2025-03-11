using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EConstructionApp.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AppUserFieldsWereAddedForRefreshToken : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "RefreshToken", "RefreshTokenEndDate", "SecurityStamp" },
                values: new object[] { "324b86d1-2bf0-4a0b-9c69-8684f5a9a7c4", "AQAAAAIAAYagAAAAEDGIx7NYvgHphwCzPiPpfd1bOD9yijxIuzsMo0dE1Nv3oD8ikubMz1jbEJsBpthUsA==", null, null, "8cf91e04-8940-4153-b94b-168b2d71c0e6" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "99D74F43-7E23-41BE-9F98-10C5D6130312",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "RefreshToken", "RefreshTokenEndDate", "SecurityStamp" },
                values: new object[] { "d2c8e713-46d3-45cc-bf76-9b8ed2afbf1c", "AQAAAAIAAYagAAAAEOH1oXF3psZQZ3+oIsypwUf0xbmnRrcmmXvhmkta0oGoMqd8CxArUbb+YjyUhvxZ/w==", null, null, "c4fa41a5-716b-4bb6-9ef7-d62041e323b8" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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
    }
}
