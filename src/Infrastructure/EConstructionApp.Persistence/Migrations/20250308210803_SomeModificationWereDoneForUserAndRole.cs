using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EConstructionApp.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class SomeModificationWereDoneForUserAndRole : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "EmailConfirmed",
                table: "AspNetUsers",
                type: "bit",
                nullable: false,
                defaultValue: false);

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
                columns: new[] { "ConcurrencyStamp", "EmailConfirmed", "PasswordHash", "SecurityStamp" },
                values: new object[] { "22ec6872-c24d-4780-8993-2f55da5b7ae3", true, "AQAAAAIAAYagAAAAEMEVWQ1WZ9URvv4Uw+/kiaJebxmC69vpC9JZjNwzek5T8x+1MLzw+F4VeHUMHG2oKg==", "997aa0c1-91c1-46b9-8da6-405c45cedba2" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "99D74F43-7E23-41BE-9F98-10C5D6130312",
                columns: new[] { "ConcurrencyStamp", "EmailConfirmed", "PasswordHash", "SecurityStamp" },
                values: new object[] { "56f7d75c-6c59-4122-9fac-cf68893566de", true, "AQAAAAIAAYagAAAAELjX37mI2rhamHeCkAAvOuQ21KBNP5mSyo6afCwjRrbwZn1/1BDzVNND6Ilw1ofYiQ==", "20e4fa8a-0dcc-44cb-8e80-baea7833b972" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EmailConfirmed",
                table: "AspNetUsers");

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
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "95688a57-e74d-4140-b00f-a1ad0428c20b", "AQAAAAIAAYagAAAAECdGIgQvN8ZuTGvZ2u4mzpIkZTftAU0r6Tg4JQApzmD1iXxdc9dPfSVzRRvwfXHAlQ==", "a665fc60-8026-46fb-86ac-4ebe24a74011" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "99D74F43-7E23-41BE-9F98-10C5D6130312",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "60935623-52c1-422c-ac62-703b83ae95c3", "AQAAAAIAAYagAAAAEImJdQJ/63tyRbMG0EhEfvEvibS79wdheuh+6ortZNV8hianUoNTVubnjq3i5qhB1g==", "62daad66-15d5-43ac-903c-07b72e5f9d5d" });
        }
    }
}
