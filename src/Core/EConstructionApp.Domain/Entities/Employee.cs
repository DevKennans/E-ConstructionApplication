using EConstructionApp.Domain.Entities.Common;
using EConstructionApp.Domain.Entities.Common.Abstractions;

namespace EConstructionApp.Domain.Entities
{
    public class Employee : BaseEntity, IActivityStatus
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateOnly DateOfBirth { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public decimal Salary { get; set; }

        public bool IsCurrentlyWorking { get; set; } = false;

        public bool IsDeleted { get; set; } = false;

        public Guid? CurrentTaskId { get; set; }
        public Task? CurrentTask { get; set; }

        public ICollection<EmployeeAttendance> EmployeeAttendances { get; set; } = new List<EmployeeAttendance>();
    }
}
