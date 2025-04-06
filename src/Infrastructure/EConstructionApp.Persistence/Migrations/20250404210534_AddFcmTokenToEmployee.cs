using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EConstructionApp.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddFcmTokenToEmployee : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DeviceToken",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "18FF4584-E530-4CFE-ADC1-9485EBCC1982",
                column: "ConcurrencyStamp",
                value: "8ff1f89c-d096-45a5-b4e1-f0c904b6a89d");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "A6975C7C-5397-4BBB-B450-2790781BCBAB",
                column: "ConcurrencyStamp",
                value: "0f1701f8-537a-4d59-a2fe-1ca97b8a94c3");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "BA54FA79-8361-4560-B1A5-C55097FE6E63",
                column: "ConcurrencyStamp",
                value: "d1b764ed-cadd-4ffd-a371-009aa03e1d27");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "43B64316-DE56-4300-AE86-C298AEA73C7B",
                columns: new[] { "ConcurrencyStamp", "DeviceToken", "PasswordHash", "SecurityStamp" },
                values: new object[] { "ed8b6ce8-a4c4-45a5-bc54-c8fc66152860", null, "AQAAAAIAAYagAAAAELB+CwWZYSUFAaXT/fnHMWhCrnkAnkZt2GW7Z916GoQf3QRju4v53fcP6t6X7bqnjA==", "5ef39a4f-7ec4-4b1c-b164-8e4978b9797c" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "99D74F43-7E23-41BE-9F98-10C5D6130312",
                columns: new[] { "ConcurrencyStamp", "DeviceToken", "PasswordHash", "SecurityStamp" },
                values: new object[] { "80114a81-2007-4e94-8c71-1270b2bd4dd1", null, "AQAAAAIAAYagAAAAEDx4lDCYZPQoV70N3FbBScwE1YOqjT55z49w47duPdDmk3aZgrP22GO8BGEZhQD5ZA==", "390a8cae-15df-494c-b9c1-a78b943dd7f8" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DeviceToken",
                table: "AspNetUsers");

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
    }
}
