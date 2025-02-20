using EConstructionApp.Domain.Entities.Common;
using System.ComponentModel.DataAnnotations.Schema;

namespace EConstructionApp.Domain.Entities
{
    public class EmployeeAttendance : BaseEntity
    {
        [NotMapped]
        public override DateTime? ModifiedDate { get => base.ModifiedDate; set => base.ModifiedDate = value; }

        public Guid EmployeeId { get; set; }
        public Employee Employee { get; set; }

        public DateTime Date { get; set; }

        public DateTime? CheckInTime { get; set; }
        public DateTime? CheckOutTime { get; set; }
    }
}
