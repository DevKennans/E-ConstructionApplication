using EConstructionApp.Domain.Entities.Common;
using EConstructionApp.Domain.Entities.Common.Abstractions;
using EConstructionApp.Domain.Entities.Cross;
using EConstructionApp.Domain.Enums.Tasks;
using TaskStatus = EConstructionApp.Domain.Enums.Tasks.TaskStatus;

namespace EConstructionApp.Domain.Entities
{
    public class Task : BaseEntity, IActivityStatus
    {
        public string AssignedBy { get; set; }
        public string AssignedByPhone { get; set; }
        public string AssignedByAddress { get; set; }

        public string Title { get; set; }
        public string Description { get; set; }
        public DateOnly Deadline { get; set; }

        public TaskPriority Priority { get; set; }
        public TaskStatus Status { get; set; } = TaskStatus.Pending;

        public bool IsDeleted { get; set; } = false;

        public ICollection<Employee> Employees { get; set; } = new List<Employee>();
        public ICollection<MaterialTask> MaterialTasks { get; set; } = new List<MaterialTask>();
    }
}
