using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EConstructionApp.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class TaskFieldsWereModified : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AssignedByEmail",
                table: "Tasks");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Task_AssignedBy_Length",
                table: "Tasks",
                sql: "LEN(AssignedBy) >= 2");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Task_AssignedByAddress_Length",
                table: "Tasks",
                sql: "LEN(AssignedByAddress) >= 5");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Task_AssignedByPhone_Length",
                table: "Tasks",
                sql: "LEN(AssignedByPhone) >= 10");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Task_Deadline",
                table: "Tasks",
                sql: "Deadline >= DATEADD(DAY, 1, GETDATE())");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Task_Description_Length",
                table: "Tasks",
                sql: "LEN(Description) >= 10");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Task_Title_Length",
                table: "Tasks",
                sql: "LEN(Title) >= 5");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_Task_AssignedBy_Length",
                table: "Tasks");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Task_AssignedByAddress_Length",
                table: "Tasks");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Task_AssignedByPhone_Length",
                table: "Tasks");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Task_Deadline",
                table: "Tasks");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Task_Description_Length",
                table: "Tasks");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Task_Title_Length",
                table: "Tasks");

            migrationBuilder.AddColumn<string>(
                name: "AssignedByEmail",
                table: "Tasks",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: false,
                defaultValue: "");
        }
    }
}
