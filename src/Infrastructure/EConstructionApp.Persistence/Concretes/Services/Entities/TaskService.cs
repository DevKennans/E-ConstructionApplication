using AutoMapper;
using EConstructionApp.Application.DTOs.Tasks;
using EConstructionApp.Application.DTOs.Tasks.Relations;
using EConstructionApp.Application.Interfaces.Services.Entities;
using EConstructionApp.Application.Interfaces.UnitOfWorks;
using EConstructionApp.Domain.Entities;
using EConstructionApp.Domain.Entities.Cross;
using Microsoft.EntityFrameworkCore;
using Task = EConstructionApp.Domain.Entities.Task;

namespace EConstructionApp.Persistence.Concretes.Services.Entities
{
    public class TaskService : ITaskService
    {
        private readonly IEmployeeService _employeeService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public TaskService(IEmployeeService employeeService, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _employeeService = employeeService;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<(bool IsSuccess, string Message)> InsertAsync(TaskInsertDto dto)
        {
            (bool IsSuccess, string Message) validationResult = ValidateTaskCreationDto(dto);
            if (!validationResult.IsSuccess)
                return (false, validationResult.Message);

            Task task = _mapper.Map<Task>(dto);

            (bool IsSuccess, string Message, IList<Employee> Employees, int AssignedCount) employeeResult = await AssignEmployeesAsync(dto.EmployeeIds!);
            if (!employeeResult.IsSuccess)
                return (false, employeeResult.Message);

            task.Employees = employeeResult.Employees;

            (bool IsSuccess, string Message, IList<MaterialTask> MaterialTasks, int AssignedCount) materialResult = await AssignMaterialsAsync(dto.MaterialAssignments!);
            if (!materialResult.IsSuccess)
                return (false, materialResult.Message);

            task.MaterialTasks = materialResult.MaterialTasks;

            await _unitOfWork.GetWriteRepository<Task>().AddAsync(task);
            await _unitOfWork.SaveAsync();

            return (true, GenerateTaskCreationSuccessMessage(task.Title, employeeResult.AssignedCount, materialResult.AssignedCount));
        }

        private (bool IsSuccess, string Message) ValidateTaskCreationDto(TaskInsertDto dto)
        {
            if (dto is null)
                return (false, "Invalid task data.");

            List<string> errors = new List<string>();

            if (string.IsNullOrWhiteSpace(dto.AssignedBy))
                errors.Add("AssignedBy is required.");
            if (string.IsNullOrWhiteSpace(dto.AssignedByPhone))
                errors.Add("AssignedByPhone is required.");
            if (string.IsNullOrWhiteSpace(dto.AssignedByEmail))
                errors.Add("AssignedByEmail is required.");
            if (string.IsNullOrWhiteSpace(dto.AssignedByAddress))
                errors.Add("AssignedByAddress is required.");

            if (string.IsNullOrWhiteSpace(dto.Title))
                errors.Add("Task title is required.");
            if (string.IsNullOrWhiteSpace(dto.Description))
                errors.Add("Task description is required.");

            if (dto.Deadline < DateOnly.FromDateTime(DateTime.UtcNow))
                errors.Add("Deadline must be a future date.");

            return errors.Any() ? (false, string.Join(" ", errors)) : (true, default!);
        }

        private async Task<(bool IsSuccess, string Message, IList<Employee> Employees, int AssignedCount)> AssignEmployeesAsync(IList<Guid> employeeIds)
        {
            if (employeeIds is null || !employeeIds.Any())
                return (true, "No employees assigned.", default!, default!);

            (bool IsSuccess, string Message, IList<Application.DTOs.Employees.EmployeeDto> AvailableEmployees) = await _employeeService.GetAvailableEmployeesListAsync();
            if (!IsSuccess)
                return (false, "No available employees found for assignment.", default!, default!);

            List<Application.DTOs.Employees.EmployeeDto> validEmployees = AvailableEmployees.Where(e => employeeIds.Contains(e.Id)).ToList();
            if (validEmployees.Count != employeeIds.Count)
                return (false, "Some employees are either inactive or already assigned to another task.", default!, default!);

            IList<Employee> employeeEntities = _mapper.Map<IList<Employee>>(validEmployees);
            foreach (Employee employee in employeeEntities)
            {
                employee.IsCurrentlyWorking = true;
                await _unitOfWork.GetWriteRepository<Employee>().UpdateAsync(employee);
            }

            return (true, default!, employeeEntities, employeeEntities.Count);
        }

        private async Task<(bool IsSuccess, string Message, IList<MaterialTask> MaterialTasks, int AssignedCount)> AssignMaterialsAsync(IList<MaterialAssignmentInsertDto> materialAssignments)
        {
            if (materialAssignments is null || !materialAssignments.Any())
                return (true, "No materials assigned.", default!, default!);

            List<MaterialAssignmentInsertDto> groupedAssignments = materialAssignments
                .GroupBy(m => m.MaterialId)
                .Select(g => new MaterialAssignmentInsertDto
                {
                    MaterialId = g.Key,
                    Quantity = g.Sum(m => m.Quantity)
                }).ToList();

            IList<Guid> materialIds = groupedAssignments.Select(m => m.MaterialId).ToList();

            IList<Material> validMaterials = await _unitOfWork.GetReadRepository<Material>().GetAllAsync(
                    enableTracking: true,
                    includeDeleted: true,
                    predicate: m => materialIds.Contains(m.Id) && !m.IsDeleted);
            if (validMaterials.Count != materialIds.Count)
                return (false, "Some materials are inactive and cannot be assigned.", default!, default!);

            List<string> errorMessages = new List<string>();
            List<MaterialTask> materialTasks = new List<MaterialTask>();

            foreach (MaterialAssignmentInsertDto assignment in groupedAssignments)
            {
                Material? material = validMaterials.FirstOrDefault(m => m.Id == assignment.MaterialId);
                if (material is null)
                    continue;

                if (assignment.Quantity <= 0)
                    errorMessages.Add($"Invalid quantity for material '{material.Name}'. Quantity must be greater than zero.");
                else if (material.StockQuantity < assignment.Quantity)
                    errorMessages.Add($"Material '{material.Name}' has insufficient stock. Available: {material.StockQuantity}, requested: {assignment.Quantity}.");
                else
                {
                    material.StockQuantity -= assignment.Quantity;
                    await _unitOfWork.GetWriteRepository<Material>().UpdateAsync(material);

                    materialTasks.Add(new MaterialTask
                    {
                        MaterialId = assignment.MaterialId,
                        Quantity = assignment.Quantity
                    });
                }
            }

            if (errorMessages.Any())
                return (false, string.Join(" ", errorMessages), default!, default!);

            return (true, "Materials assigned successfully.", materialTasks, materialTasks.Count);
        }

        private string GenerateTaskCreationSuccessMessage(string taskTitle, int employeeCount, int materialCount)
        {
            string message = $"Task '{taskTitle}' has been successfully created.";
            List<string> details = new();

            if (employeeCount == 1)
                details.Add("1 employee assigned");
            else if (employeeCount > 1)
                details.Add($"{employeeCount} employees assigned");

            if (materialCount == 1)
                details.Add("1 material assigned");
            else if (materialCount > 1)
                details.Add($"{materialCount} materials assigned");

            if (details.Any())
                message += $" ({string.Join(", ", details)}).";

            return message;
        }

        public async Task<(bool IsSuccess, string Message)> UpdateTaskDetailsAsync(TaskDetailsUpdateDto dto)
        {
            (bool IsSuccess, string Message, Task? Task) = await GetTaskWithRelationsByIdAsync(dto.Id);
            if (!IsSuccess)
                return (false, Message);

            (bool IsSuccess, string Message) validation = ValidateTaskUpdate(dto, Task!);
            if (!validation.IsSuccess)
                return (false, validation.Message);

            List<string> updates = ApplyTaskUpdates(dto, Task!);
            if (!updates.Any())
                return (false, "No changes detected. Task details remain the same.");

            await _unitOfWork.GetWriteRepository<Task>().UpdateAsync(Task!);
            await _unitOfWork.SaveAsync();

            return (true, $"Task '{Task!.Title}' updated successfully. {string.Join(" ", updates)}");
        }

        private (bool IsSuccess, string Message) ValidateTaskUpdate(TaskDetailsUpdateDto dto, Task task)
        {
            if (dto is null)
                return (false, "Invalid task update data.");

            if (dto.Deadline < DateOnly.FromDateTime(DateTime.UtcNow))
                return (false, "Deadline cannot be a past date.");

            return (true, default!);
        }

        private List<string> ApplyTaskUpdates(TaskDetailsUpdateDto dto, Task task)
        {
            List<string> updates = new List<string>();

            void UpdateField<T>(T newValue, T oldValue, Action<T> setter, string fieldName)
            {
                if (!EqualityComparer<T>.Default.Equals(newValue, oldValue))
                {
                    setter(newValue);
                    updates.Add($"{fieldName} updated.");
                }
            }

            UpdateField(dto.AssignedBy, task.AssignedBy, newValue => task.AssignedBy = newValue, "AssignedBy");
            UpdateField(dto.AssignedByPhone, task.AssignedByPhone, newValue => task.AssignedByPhone = newValue, "AssignedByPhone");
            UpdateField(dto.AssignedByEmail, task.AssignedByEmail, newValue => task.AssignedByEmail = newValue, "AssignedByEmail");
            UpdateField(dto.AssignedByAddress, task.AssignedByAddress, newValue => task.AssignedByAddress = newValue, "AssignedByAddress");
            UpdateField(dto.Title, task.Title, newValue => task.Title = newValue, "Title");
            UpdateField(dto.Description, task.Description, newValue => task.Description = newValue, "Description");
            UpdateField(dto.Deadline, task.Deadline, newValue => task.Deadline = newValue, "Deadline");
            UpdateField(dto.Priority, task.Priority, newValue => task.Priority = newValue, "Priority");
            UpdateField(dto.Status, task.Status, newValue => task.Status = newValue, "Status");

            return updates;
        }

        public async Task<(bool IsSuccess, string Message, int ActiveTasks, int TotalTasks)> GetTaskCountsAsync()
        {
            int totalTasks = await _unitOfWork.GetReadRepository<Task>().CountAsync(includeDeleted: true);
            if (totalTasks == 0)
                return (false, "No tasks found.", 0, 0);

            int activeTasks = await _unitOfWork.GetReadRepository<Task>().CountAsync(
                includeDeleted: false,
                predicate: c => !c.IsDeleted);

            return (true, "Task counts retrieved successfully.", activeTasks, totalTasks);
        }

        private async Task<(bool IsSuccess, string Message, Task? TaskEntity)> GetTaskWithRelationsByIdAsync(Guid taskId)
        {
            Task? task = await _unitOfWork.GetReadRepository<Task>().GetAsync(
                predicate: t => t.Id == taskId && !t.IsDeleted,
                include: q => q
                    .Include(t => t.Employees)
                    .Include(t => t.MaterialTasks)
                        .ThenInclude(mt => mt.Material)
                            .ThenInclude(m => m.Category));
            if (task is null)
                return (false, $"Task with ID '{taskId}' not found or has been deleted.", default!);

            return (true, "Task retrieved successfully.", task);
        }

        /* GetAllActiveTasksListAsync method can use for only and only active list. */
        public async Task<(bool IsSuccess, string Message, IList<TaskDto> Tasks)> GetAllActiveTasksListAsync()
        {
            IList<Task> tasks = await _unitOfWork.GetReadRepository<Task>().GetAllAsync(
                    enableTracking: false,
                    includeDeleted: true,
                    predicate: t => !t.IsDeleted,
                    include: q => q
                        .Include(t => t.Employees)
                        .Include(t => t.MaterialTasks)
                            .ThenInclude(mt => mt.Material)
                                .ThenInclude(m => m.Category),
                    orderBy: q => q.OrderByDescending(t => t.InsertedDate));
            if (!tasks.Any())
                return (false, "No active tasks found.", default!);

            IList<TaskDto> taskDtos = _mapper.Map<IList<TaskDto>>(tasks);
            foreach (TaskDto taskDto in taskDtos)
            {
                Task? correspondingTask = tasks.FirstOrDefault(t => t.Id == taskDto.Id);
                if (correspondingTask is null)
                    continue;

                decimal totalCost = correspondingTask.MaterialTasks
                    .Where(mt => mt.Material is not null)
                    .Sum(mt => mt.Quantity * mt.Material.Price);

                taskDto.TotalCost = totalCost;
            }

            return (true, "Active tasks retrieved successfully.", taskDtos);
        }

        public async Task<(bool IsSuccess, string Message, IList<TaskStatusCountsDto> TaskCounts)> GetTaskCountsByStatusAsync()
        {
            IList<Task> taskCounts = await _unitOfWork.GetReadRepository<Task>()
                .GetAllAsync(
                    enableTracking: false,
                    includeDeleted: false,
                    predicate: t => !t.IsDeleted);
            if (!taskCounts.Any())
                return (false, "No active tasks found.", default!);

            List<TaskStatusCountsDto> statusCounts = taskCounts
                .GroupBy(t => t.Status)
                .Select(group => new TaskStatusCountsDto
                {
                    Status = (TaskStatus)group.Key,
                    Count = group.Count()
                }).ToList();
            return (true, "Task status counts retrieved successfully.", statusCounts);
        }
    }
}
