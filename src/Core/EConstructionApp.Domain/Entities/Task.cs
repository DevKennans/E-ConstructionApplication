using EConstructionApp.Domain.Entities.Common;
using EConstructionApp.Domain.Entities.Common.Abstractions;
using EConstructionApp.Domain.Entities.Cross;

namespace EConstructionApp.Domain.Entities
{
    public class Task : BaseEntity, ITaskStatus
    {
        public string Description { get; set; }

        public bool IsActiveTask { get; set; } = true;

        public ICollection<EmployeeTask> EmployeeTasks { get; set; }
        public ICollection<MaterialTask> MaterialTasks { get; set; }
    }
}
