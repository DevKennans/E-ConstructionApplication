using EConstructionApp.Domain.Entities;
using EConstructionApp.Domain.Entities.Cross;
using EConstructionApp.Domain.Enums;
using EConstructionApp.Persistence.Contexts;
using Microsoft.AspNetCore.Mvc;

namespace EConstructionApp.WebAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class DataSeedController : ControllerBase
    {
        private readonly EConstructionDbContext _dbContext;
        public DataSeedController(EConstructionDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpPost]
        public IActionResult SeedData()
        {
            if (_dbContext.Materials.Any() || _dbContext.Employees.Any() || _dbContext.Tasks.Any())
                return BadRequest("Database already contains data!");

            Category category1 = new Category { Id = Guid.NewGuid(), Name = "Construction Materials" };
            Category category2 = new Category { Id = Guid.NewGuid(), Name = "Finishing Materials" };

            _dbContext.Categories.AddRange(category1, category2);

            Material material1 = new Material
            {
                Id = Guid.NewGuid(),
                Name = "Cement",
                Price = 12.50m,
                StockQuantity = 1000.5m,
                Measure = Measure.Kilogram,
                CategoryId = category1.Id
            };

            Material material2 = new Material
            {
                Id = Guid.NewGuid(),
                Name = "Bricks",
                Price = 1.20m,
                StockQuantity = 500,
                Measure = Measure.Unit,
                CategoryId = category1.Id
            };

            Material material3 = new Material
            {
                Id = Guid.NewGuid(),
                Name = "Steel Rod",
                Price = 8.75m,
                StockQuantity = 250.3m,
                Measure = Measure.Meter,
                CategoryId = category2.Id
            };

            _dbContext.Materials.AddRange(material1, material2, material3);

            Employee employee1 = new Employee
            {
                Id = Guid.NewGuid(),
                FirstName = "John",
                LastName = "Doe",
                DateOfBirth = new DateTime(1990, 5, 10),
                PhoneNumber = "1234567890",
                Address = "123 Main St",
                IsCurrentlyWorking = true
            };

            Employee employee2 = new Employee
            {
                Id = Guid.NewGuid(),
                FirstName = "Jane",
                LastName = "Smith",
                DateOfBirth = new DateTime(1985, 8, 25),
                PhoneNumber = "9876543210",
                Address = "456 Elm St",
                IsCurrentlyWorking = true
            };

            Employee employee3 = new Employee
            {
                Id = Guid.NewGuid(),
                FirstName = "Alex",
                LastName = "Brown",
                DateOfBirth = new DateTime(1992, 3, 15),
                PhoneNumber = "5556667777",
                Address = "789 Oak St",
                IsCurrentlyWorking = true
            };

            _dbContext.Employees.AddRange(employee1, employee2, employee3);

            EmployeeAttendance attendance1 = new EmployeeAttendance
            {
                Id = Guid.NewGuid(),
                EmployeeId = employee1.Id,
                Dairy = DateOnly.FromDateTime(DateTime.Now),
                CheckInTime = DateTime.Now.AddHours(-4),
                CheckOutTime = DateTime.Now
            };

            EmployeeAttendance attendance2 = new EmployeeAttendance
            {
                Id = Guid.NewGuid(),
                EmployeeId = employee2.Id,
                Dairy = DateOnly.FromDateTime(DateTime.Now),
                CheckInTime = DateTime.Now.AddHours(-3),
                CheckOutTime = DateTime.Now
            };

            EmployeeAttendance attendance3 = new EmployeeAttendance
            {
                Id = Guid.NewGuid(),
                EmployeeId = employee3.Id,
                Dairy = DateOnly.FromDateTime(DateTime.Now),
                CheckInTime = DateTime.Now.AddHours(-5),
                CheckOutTime = DateTime.Now
            };

            _dbContext.EmployeeAttendances.AddRange(attendance1, attendance2, attendance3);

            Domain.Entities.Task task1 = new Domain.Entities.Task
            {
                Id = Guid.NewGuid(),
                Description = "Build foundation",
                IsDone = false
            };

            Domain.Entities.Task task2 = new Domain.Entities.Task
            {
                Id = Guid.NewGuid(),
                Description = "Wall construction",
                IsDone = false
            };

            _dbContext.Tasks.AddRange(task1, task2);

            task1.Employees.Add(employee1);
            task1.Employees.Add(employee3);
            task2.Employees.Add(employee2);

            MaterialTask materialTask1 = new MaterialTask
            {
                MaterialId = material1.Id,
                TaskId = task1.Id,
                Quantity = 500.5m
            };

            MaterialTask materialTask2 = new MaterialTask
            {
                MaterialId = material3.Id,
                TaskId = task1.Id,
                Quantity = 100.0m
            };

            MaterialTask materialTask3 = new MaterialTask
            {
                MaterialId = material2.Id,
                TaskId = task2.Id,
                Quantity = 300
            };

            MaterialTask materialTask4 = new MaterialTask
            {
                MaterialId = material3.Id,
                TaskId = task2.Id,
                Quantity = 50.3m
            };

            _dbContext.MaterialTasks.AddRange(materialTask1, materialTask2, materialTask3, materialTask4);
            _dbContext.SaveChanges();

            return Ok("Database seeded successfully!");
        }
    }
}
