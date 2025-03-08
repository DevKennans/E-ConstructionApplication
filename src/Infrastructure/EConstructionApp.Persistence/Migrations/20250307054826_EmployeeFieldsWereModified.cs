using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EConstructionApp.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class EmployeeFieldsWereModified : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Role",
                table: "Employees",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddCheckConstraint(
                name: "CK_Employee_FirstName_Length",
                table: "Employees",
                sql: "LEN(FirstName) >= 2");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Employee_LastName_Length",
                table: "Employees",
                sql: "LEN(LastName) >= 2");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Employee_Min_Age",
                table: "Employees",
                sql: "DateOfBirth <= DATEADD(YEAR, -18, GETDATE())");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Employee_PhoneNumber_Length",
                table: "Employees",
                sql: "LEN(PhoneNumber) >= 10");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Employee_Salary",
                table: "Employees",
                sql: "Salary >= 0");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_Employee_FirstName_Length",
                table: "Employees");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Employee_LastName_Length",
                table: "Employees");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Employee_Min_Age",
                table: "Employees");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Employee_PhoneNumber_Length",
                table: "Employees");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Employee_Salary",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "Role",
                table: "Employees");
        }
    }
}
