namespace EConstructionApp.Application.DTOs.Employees
{
    public class EmployeeDto
    {
        public Guid Id { get; set; }
        public virtual DateTime InsertedDate { get; set; }
        public virtual DateTime? ModifiedDate { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateOnly DateOfBirth { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public decimal Salary { get; set; }

        public bool IsCurrentlyWorking { get; set; }

        public bool IsDeleted { get; set; }
    }
}
