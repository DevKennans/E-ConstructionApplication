namespace EConstructionApp.Domain.Entities.Cross
{
    public class EmployeeTask
    {
        public Guid EmployeeId { get; set; }
        public Employee Employee { get; set; }

        public Guid TaskId { get; set; }
        public Task Task { get; set; }
    }
}
