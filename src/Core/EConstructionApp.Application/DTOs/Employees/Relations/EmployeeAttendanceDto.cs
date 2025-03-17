using EConstructionApp.Domain.Enums.Employees;

namespace EConstructionApp.Application.DTOs.Employees.Relations
{
    public class EmployeeAttendanceDto
    {
        public string EmployeeFullName { get; set; }
        public EmployeeRole Role { get; set; }
        public string PhoneNumber { get; set; }

        public DateOnly Dairy { get; set; }

        public DateTime? CheckInTime { get; set; }
        public DateTime? CheckOutTime { get; set; }
    }
}
