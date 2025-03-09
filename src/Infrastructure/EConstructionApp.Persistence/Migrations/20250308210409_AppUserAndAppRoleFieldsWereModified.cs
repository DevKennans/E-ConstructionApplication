using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EConstructionApp.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AppUserAndAppRoleFieldsWereModified : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PhoneNumber",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "PhoneNumberConfirmed",
                table: "AspNetUsers",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "18FF4584-E530-4CFE-ADC1-9485EBCC1982",
                column: "ConcurrencyStamp",
                value: "af7007cc-0468-4df1-af10-a4288d9128a5");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "A6975C7C-5397-4BBB-B450-2790781BCBAB",
                column: "ConcurrencyStamp",
                value: "68367601-fd2c-451b-b3ea-d873eab6472f");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "BA54FA79-8361-4560-B1A5-C55097FE6E63",
                column: "ConcurrencyStamp",
                value: "3755c161-cbf8-4d2c-b733-b5e1e2896acb");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "43B64316-DE56-4300-AE86-C298AEA73C7B",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp" },
                values: new object[] { "95688a57-e74d-4140-b00f-a1ad0428c20b", "AQAAAAIAAYagAAAAECdGIgQvN8ZuTGvZ2u4mzpIkZTftAU0r6Tg4JQApzmD1iXxdc9dPfSVzRRvwfXHAlQ==", null, true, "a665fc60-8026-46fb-86ac-4ebe24a74011" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "99D74F43-7E23-41BE-9F98-10C5D6130312",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp" },
                values: new object[] { "60935623-52c1-422c-ac62-703b83ae95c3", "AQAAAAIAAYagAAAAEImJdQJ/63tyRbMG0EhEfvEvibS79wdheuh+6ortZNV8hianUoNTVubnjq3i5qhB1g==", null, true, "62daad66-15d5-43ac-903c-07b72e5f9d5d" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PhoneNumber",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "PhoneNumberConfirmed",
                table: "AspNetUsers");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "18FF4584-E530-4CFE-ADC1-9485EBCC1982",
                column: "ConcurrencyStamp",
                value: "23515f07-ff2f-4b22-9b29-32cc82121fee");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "A6975C7C-5397-4BBB-B450-2790781BCBAB",
                column: "ConcurrencyStamp",
                value: "21cec4c2-df7a-4d80-88f9-d2e02541efee");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "BA54FA79-8361-4560-B1A5-C55097FE6E63",
                column: "ConcurrencyStamp",
                value: "0cfbf09b-70ce-42a7-83d4-5d8e0c57ceb7");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "43B64316-DE56-4300-AE86-C298AEA73C7B",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "4edfcb6c-9e53-45c1-b3fb-4651593529bb", "AQAAAAIAAYagAAAAEMATRBcezUcRyin+SxU2KCMwi8tGoGgQdsJ8/PHp6yRaSsT4iULKghjwiOtD9DGxkw==", "e27a6493-b348-43ef-a432-9b672026f8cf" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "99D74F43-7E23-41BE-9F98-10C5D6130312",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "b2762fe9-a140-48cf-a2f7-97271bd77425", "AQAAAAIAAYagAAAAEK9jjgQEmdXkfVugq7aladxSEbU+GnWjMNY1d5EkW9ZJNY6u8QX8F1GdzimEGyFplw==", "92ae7ca7-c440-4433-9cc4-d0c3b9276cde" });
        }
    }
}
