using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EConstructionApp.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class RefreshTokenPartsWereInserted : Migration
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
                value: "b98cdef3-9386-4465-8305-8cba081e75a0");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "A6975C7C-5397-4BBB-B450-2790781BCBAB",
                column: "ConcurrencyStamp",
                value: "914f00a9-5209-4cc3-b3c2-9ab396543aaa");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "BA54FA79-8361-4560-B1A5-C55097FE6E63",
                column: "ConcurrencyStamp",
                value: "a92858e1-9f2e-4ccc-b5e2-b648d655d887");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "43B64316-DE56-4300-AE86-C298AEA73C7B",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "22ec6872-c24d-4780-8993-2f55da5b7ae3", "AQAAAAIAAYagAAAAEMEVWQ1WZ9URvv4Uw+/kiaJebxmC69vpC9JZjNwzek5T8x+1MLzw+F4VeHUMHG2oKg==", "997aa0c1-91c1-46b9-8da6-405c45cedba2" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "99D74F43-7E23-41BE-9F98-10C5D6130312",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "56f7d75c-6c59-4122-9fac-cf68893566de", "AQAAAAIAAYagAAAAELjX37mI2rhamHeCkAAvOuQ21KBNP5mSyo6afCwjRrbwZn1/1BDzVNND6Ilw1ofYiQ==", "20e4fa8a-0dcc-44cb-8e80-baea7833b972" });
        }
    }
}
