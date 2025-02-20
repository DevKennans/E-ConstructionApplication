using EConstructionApp.Domain.Entities.Common;
using EConstructionApp.Domain.Entities.Common.Abstractions;
using EConstructionApp.Domain.Entities.Cross;

namespace EConstructionApp.Domain.Entities
{
    public class Task : BaseEntity, IActivityStatus
    {
        public string Description { get; set; }

        public bool IsDone { get; set; } = false;

        public bool IsDeleted { get; set; } = false;

        public ICollection<Employee> Employees { get; set; } = new List<Employee>();
        public ICollection<MaterialTask> MaterialTasks { get; set; }
    }
}
