using EConstructionApp.Domain.Enums.Employees;

namespace EConstructionApp.Application.DTOs.Employees
{
    public class EmployeeInsertDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateOnly DateOfBirth { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public decimal Salary { get; set; }

        public EmployeeRole Role { get; set; }
    }
}
