using EConstructionApp.Domain.Enums.Tasks;
using TaskStatus = EConstructionApp.Domain.Enums.Tasks.TaskStatus;

namespace EConstructionApp.Application.DTOs.Tasks
{
    public class TaskDetailsUpdateDto
    {
        public Guid Id { get; set; }

        public string AssignedBy { get; set; }
        public string AssignedByPhone { get; set; }
        public string AssignedByEmail { get; set; }
        public string AssignedByAddress { get; set; }

        public string Title { get; set; }
        public string Description { get; set; }
        public DateOnly Deadline { get; set; }

        public TaskPriority Priority { get; set; }
        public TaskStatus Status { get; set; }
    }
}
