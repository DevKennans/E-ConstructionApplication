using EConstructionApp.Domain.Entities.Common;

namespace EConstructionApp.Domain.Entities
{
    public class EmployeeAttendance : BaseEntity
    {
        public Guid EmployeeId { get; set; }
        public Employee Employee { get; set; }

        public DateOnly Dairy { get; set; }

        public DateTime? CheckInTime { get; set; }
        public DateTime? CheckOutTime { get; set; }
    }
}
