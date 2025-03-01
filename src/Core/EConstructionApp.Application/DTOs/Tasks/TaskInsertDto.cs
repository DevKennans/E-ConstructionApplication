using EConstructionApp.Application.DTOs.Tasks.Relations;
using EConstructionApp.Domain.Enums.Tasks;

namespace EConstructionApp.Application.DTOs.Tasks
{
    public class TaskInsertDto
    {
        public string AssignedBy { get; set; }
        public string AssignedByPhone { get; set; }
        public string AssignedByEmail { get; set; }
        public string AssignedByAddress { get; set; }

        public string Title { get; set; }
        public string Description { get; set; }
        public DateOnly Deadline { get; set; }

        public TaskPriority Priority { get; set; }

        public List<Guid>? EmployeeIds { get; set; }
        public List<MaterialAssignmentInsertDto>? MaterialAssignments { get; set; }
    }
}
