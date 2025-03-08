using EConstructionApp.Application.DTOs.Employees;
using EConstructionApp.Application.DTOs.Tasks.Relations;
using EConstructionApp.Domain.Enums.Tasks;
using TaskStatus = EConstructionApp.Domain.Enums.Tasks.TaskStatus;

namespace EConstructionApp.Application.DTOs.Tasks
{
    public class TaskDto
    {
        public Guid Id { get; set; }
        public virtual DateTime InsertedDate { get; set; }
        public virtual DateTime? ModifiedDate { get; set; }

        public string AssignedBy { get; set; }
        public string AssignedByPhone { get; set; }
        public string AssignedByAddress { get; set; }

        public string Title { get; set; }
        public string Description { get; set; }
        public DateOnly Deadline { get; set; }

        public TaskPriority Priority { get; set; }
        public TaskStatus Status { get; set; }

        public decimal TotalCost { get; set; }

        public bool IsDeleted { get; set; }

        public IList<EmployeeDto> Employees { get; set; } = new List<EmployeeDto>();
        public IList<TaskMaterialsDto> MaterialAssignments { get; set; } = new List<TaskMaterialsDto>();
    }
}
