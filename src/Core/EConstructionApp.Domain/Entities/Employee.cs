using EConstructionApp.Domain.Entities.Common;
using EConstructionApp.Domain.Entities.Common.Abstractions;
using EConstructionApp.Domain.Entities.Cross;

namespace EConstructionApp.Domain.Entities
{
    public class Employee : BaseEntity, IEmployeeStatus
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }

        public bool IsActiveEmployee { get; set; } = true;

        public ICollection<EmployeeTask> EmployeeTasks { get; set; }
        public ICollection<EmployeeAttendance> EmployeeAttendances { get; set; }
    }
}
