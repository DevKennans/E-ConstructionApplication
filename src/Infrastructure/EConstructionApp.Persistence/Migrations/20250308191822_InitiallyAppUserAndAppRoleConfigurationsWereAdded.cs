using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace EConstructionApp.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitiallyAppUserAndAppRoleConfigurationsWereAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "LastName",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "FirstName",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "18FF4584-E530-4CFE-ADC1-9485EBCC1982", null, "Admin", "ADMIN" },
                    { "A6975C7C-5397-4BBB-B450-2790781BCBAB", null, "Employee", "EMPLOYEE" },
                    { "BA54FA79-8361-4560-B1A5-C55097FE6E63", null, "Moderator", "MODERATOR" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Email", "EmailConfirmed", "FirstName", "LastName", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[,]
                {
                    { "43B64316-DE56-4300-AE86-C298AEA73C7B", 0, "e3e6537c-5e10-4cac-8d88-5cb12e68a9f8", null, false, null, null, false, null, null, "ADMIN", "AQAAAAIAAYagAAAAEJBxLE33M+Ajh85hf+qqYNszx+uVF7gGUyc7KzEfWCq1Mg2bmBCL4spd4jSgNemQEg==", null, false, null, false, "admin" },
                    { "99D74F43-7E23-41BE-9F98-10C5D6130312", 0, "100d28f9-041e-40c5-b75f-b3aea07c1c33", null, false, null, null, false, null, null, "MODERATOR", "AQAAAAIAAYagAAAAEG3KSwX1pbA15ThGycnH91znHop47RGjKZEHevoAMiurXBlv1bx1XcAPu3eNlqG+IA==", null, false, null, false, "moderator" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[,]
                {
                    { "18FF4584-E530-4CFE-ADC1-9485EBCC1982", "43B64316-DE56-4300-AE86-C298AEA73C7B" },
                    { "BA54FA79-8361-4560-B1A5-C55097FE6E63", "99D74F43-7E23-41BE-9F98-10C5D6130312" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "A6975C7C-5397-4BBB-B450-2790781BCBAB");

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "18FF4584-E530-4CFE-ADC1-9485EBCC1982", "43B64316-DE56-4300-AE86-C298AEA73C7B" });

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "BA54FA79-8361-4560-B1A5-C55097FE6E63", "99D74F43-7E23-41BE-9F98-10C5D6130312" });

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "18FF4584-E530-4CFE-ADC1-9485EBCC1982");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "BA54FA79-8361-4560-B1A5-C55097FE6E63");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "43B64316-DE56-4300-AE86-C298AEA73C7B");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "99D74F43-7E23-41BE-9F98-10C5D6130312");

            migrationBuilder.AlterColumn<string>(
                name: "LastName",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "FirstName",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);
        }
    }
}
