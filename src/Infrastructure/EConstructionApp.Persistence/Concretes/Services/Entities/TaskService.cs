using AutoMapper;
using EConstructionApp.Application.DTOs.Tasks;
using EConstructionApp.Application.DTOs.Tasks.Relations;
using EConstructionApp.Application.Interfaces.Services.Entities;
using EConstructionApp.Application.Interfaces.UnitOfWorks;
using EConstructionApp.Domain.Entities;
using EConstructionApp.Domain.Entities.Cross;
using EConstructionApp.Persistence.Concretes.Services.Entities.Helpers;
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

        public async Task<(bool IsSuccess, string Message)> InsertAsync(TaskInsertDto? taskInsertDto)
        {
            (bool IsSuccess, string Message) validationResult = TaskServiceHelper.ValidateTaskCreationDto(taskInsertDto!);
            if (!validationResult.IsSuccess)
                return (false, validationResult.Message);

            Task task = _mapper.Map<Task>(taskInsertDto);

            (bool IsSuccess, string Message, IList<Employee>? Employees, int AssignedCount) employeeResult = await AssignEmployeesAsync(taskInsertDto!.EmployeeIds!);
            if (!employeeResult.IsSuccess)
                return (false, employeeResult.Message);
            task.Employees = employeeResult.Employees!;

            (bool IsSuccess, string Message, IList<MaterialTask>? MaterialTasks, int AssignedCount) materialResult = await AssignMaterialsAsync(taskInsertDto.MaterialAssignments!);
            if (!materialResult.IsSuccess)
                return (false, materialResult.Message);
            task.MaterialTasks = materialResult.MaterialTasks!;

            await _unitOfWork.GetWriteRepository<Task>().AddAsync(task);
            await _unitOfWork.SaveAsync();

            return (true, TaskServiceHelper.GenerateTaskCreationSuccessMessage(task.Title, employeeResult.AssignedCount, materialResult.AssignedCount));
        }

        private async Task<(bool IsSuccess, string Message, IList<Employee>? Employees, int AssignedCount)> AssignEmployeesAsync(IList<Guid> employeeIds)
        {
            if (employeeIds is null || !employeeIds.Any())
                return (true, "No employees have been assigned to the task.", default!, default!);

            (bool IsSuccess, string Message, IList<Application.DTOs.Employees.EmployeeDto> AvailableEmployees) = await _employeeService.GetAvailableEmployeesListAsync();
            if (!IsSuccess)
                return (false, "There are no available employees to assign at the moment.", default!, default!);

            List<Application.DTOs.Employees.EmployeeDto> validEmployees = AvailableEmployees.Where(e => employeeIds.Contains(e.Id)).ToList();
            if (validEmployees.Count != employeeIds.Count)
                return (false, "Some of the employees are either unavailable or already assigned to other tasks.", default!, default!);

            IList<Employee> employeeEntities = _mapper.Map<IList<Employee>>(validEmployees);
            foreach (Employee employee in employeeEntities)
            {
                employee.IsCurrentlyWorking = true;
                await _unitOfWork.GetWriteRepository<Employee>().UpdateAsync(employee);
            }

            return (true, "Employees have been successfully assigned to the task.", employeeEntities, employeeEntities.Count);
        }

        private async Task<(bool IsSuccess, string Message, IList<MaterialTask>? MaterialTasks, int AssignedCount)> AssignMaterialsAsync(IList<MaterialAssignmentInsertDto> materialAssignments)
        {
            if (materialAssignments is null || !materialAssignments.Any())
                return (true, "No materials have been assigned to the task.", default!, default!);

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
                return (false, "Some materials are either inactive or cannot be assigned at the moment.", default!, default!);

            List<string> errorMessages = new List<string>();
            List<MaterialTask> materialTasks = new List<MaterialTask>();

            foreach (MaterialAssignmentInsertDto assignment in groupedAssignments)
            {
                Material? material = validMaterials.FirstOrDefault(m => m.Id == assignment.MaterialId);
                if (material is null)
                    continue;

                if (assignment.Quantity <= 0)
                    errorMessages.Add($"The quantity for material '{material.Name}' is invalid. Please specify a quantity greater than zero.");
                else if (material.StockQuantity < assignment.Quantity)
                    errorMessages.Add($"Insufficient stock for material '{material.Name}'. Insufficient stock. Material '{material.Name}' has {material.StockQuantity} available, but {assignment.Quantity} was requested.");
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

            return (true, "Materials have been successfully assigned to the task.", materialTasks, materialTasks.Count);
        }

        public async Task<(bool IsSuccess, string Message)> UpdateTaskDetailsAsync(TaskDetailsUpdateDto? taskDetailsUpdateDto)
        {
            (bool IsSuccess, string Message) validation = TaskServiceHelper.ValidateTaskUpdateDto(taskDetailsUpdateDto);
            if (!validation.IsSuccess)
                return (false, validation.Message);

            (bool IsSuccess, string Message, Task? Task) = await GetTaskWithRelationsByIdAsync(taskDetailsUpdateDto!.Id);
            if (!IsSuccess)
                return (false, Message);

            List<string> updates = ApplyTaskUpdates(taskDetailsUpdateDto, Task!);
            if (!updates.Any())
                return (false, "No updates detected. The task details remain unchanged.");

            await _unitOfWork.GetWriteRepository<Task>().UpdateAsync(Task!);
            await _unitOfWork.SaveAsync();

            return (true, $"Task '{Task!.Title}' has been updated successfully! {string.Join(" ", updates)}");
        }

        private List<string> ApplyTaskUpdates(TaskDetailsUpdateDto taskDetailsUpdateDto, Task task)
        {
            List<string> updates = new List<string>();

            void UpdateField<T>(T newValue, T oldValue, Action<T> setter, string fieldName)
            {
                if (!EqualityComparer<T>.Default.Equals(newValue, oldValue))
                {
                    setter(newValue);
                    updates.Add($"{fieldName} has been updated.");
                }
            }

            UpdateField(taskDetailsUpdateDto.AssignedBy, task.AssignedBy, newValue => task.AssignedBy = newValue, "AssignedBy");
            UpdateField(taskDetailsUpdateDto.AssignedByPhone, task.AssignedByPhone, newValue => task.AssignedByPhone = newValue, "AssignedByPhone");
            UpdateField(taskDetailsUpdateDto.AssignedByEmail, task.AssignedByEmail, newValue => task.AssignedByEmail = newValue, "AssignedByEmail");
            UpdateField(taskDetailsUpdateDto.AssignedByAddress, task.AssignedByAddress, newValue => task.AssignedByAddress = newValue, "AssignedByAddress");
            UpdateField(taskDetailsUpdateDto.Title, task.Title, newValue => task.Title = newValue, "Title");
            UpdateField(taskDetailsUpdateDto.Description, task.Description, newValue => task.Description = newValue, "Description");
            UpdateField(taskDetailsUpdateDto.Deadline, task.Deadline, newValue => task.Deadline = newValue, "Deadline");
            UpdateField(taskDetailsUpdateDto.Priority, task.Priority, newValue => task.Priority = newValue, "Priority");
            UpdateField(taskDetailsUpdateDto.Status, task.Status, newValue => task.Status = newValue, "Status");

            return updates;
        }

        public async Task<(bool IsSuccess, string Message)> UpdateTaskEmployeesAsync(Guid taskId, List<Guid> updatedEmployeeIds)
        {
            (bool IsSuccess, string Message, Task? Task) = await GetTaskWithRelationsByIdAsync(taskId);
            if (!IsSuccess)
                return (false, Message);

            List<Guid> currentEmployeeIds = Task!.Employees.Select(e => e.Id).ToList();

            List<Guid> employeesToRemove = currentEmployeeIds.Except(updatedEmployeeIds).ToList();
            List<Guid> employeesToAdd = updatedEmployeeIds.Except(currentEmployeeIds).ToList();

            if (!employeesToRemove.Any() && !employeesToAdd.Any())
                return (true, "No changes detected. Task employees remain the same.");

            IList<Employee> employeesToBeRemoved = new List<Employee>();
            IList<Employee> employeesToBeAdded = new List<Employee>();

            if (employeesToRemove.Any())
            {
                employeesToBeRemoved = await _unitOfWork.GetReadRepository<Employee>().GetAllAsync(
                        enableTracking: true,
                        includeDeleted: false,
                        predicate: e => employeesToRemove.Contains(e.Id) && e.IsCurrentlyWorking);
            }
            if (employeesToAdd.Any())
            {
                employeesToBeAdded = await _unitOfWork.GetReadRepository<Employee>().GetAllAsync(
                        enableTracking: true,
                        includeDeleted: false,
                        predicate: e => employeesToAdd.Contains(e.Id) && !e.IsCurrentlyWorking);
            }

            int removedCount = 0, addedCount = 0;

            foreach (Employee emp in employeesToBeRemoved)
            {
                emp.IsCurrentlyWorking = false;
                Task.Employees.Remove(emp);

                removedCount++;
            }
            foreach (Employee emp in employeesToBeAdded)
            {
                emp.IsCurrentlyWorking = true;
                Task.Employees.Add(emp);

                addedCount++;
            }

            await _unitOfWork.SaveAsync();

            foreach (Employee emp in employeesToBeRemoved)
                UpdateEmployeeTaskRelationship(emp, null);
            foreach (Employee emp in employeesToBeAdded)
                UpdateEmployeeTaskRelationship(emp, taskId);

            await _unitOfWork.SaveAsync();

            return (true, GenerateTaskEmployeeUpdateMessage(addedCount, removedCount));
        }

        private void UpdateEmployeeTaskRelationship(Employee employee, Guid? taskId)
        {
            employee.CurrentTaskId = taskId;
        }

        private string GenerateTaskEmployeeUpdateMessage(int addedCount, int removedCount)
        {
            string addedMsg = addedCount > 0 ? $"{addedCount} employee{(addedCount > 1 ? "s" : "")} added" : "";
            string removedMsg = removedCount > 0 ? $"{removedCount} employee{(removedCount > 1 ? "s" : "")} removed" : "";

            string resultMessage = $"{(addedMsg + (addedMsg != "" && removedMsg != "" ? ", " : "") + removedMsg).Trim()}.";

            return $"Task employees updated successfully. {resultMessage}";
        }

        public async Task<(bool IsSuccess, string Message, int ActiveTasks, int TotalTasks)> GetTasksCountsAsync()
        {
            int totalTasks = await _unitOfWork.GetReadRepository<Task>().CountAsync(includeDeleted: true);
            if (totalTasks == 0)
                return (false, "No tasks found in the system. Please ensure tasks are added or check your filters", default!, default!);

            int activeTasks = await _unitOfWork.GetReadRepository<Task>().CountAsync(
            includeDeleted: false);

            return (true, "Tasks' counts have been successfully retrieved.", activeTasks, totalTasks);
        }

        private async Task<(bool IsSuccess, string Message, Task? TaskEntity)> GetTaskWithRelationsByIdAsync(Guid taskId)
        {
            Task? task = await _unitOfWork.GetReadRepository<Task>().GetAsync(
                enableTracking: false,
                includeDeleted: false,
                predicate: t => t.Id == taskId,
                include: q => q
                    .Include(t => t.Employees)
                    .Include(t => t.MaterialTasks)
                        .ThenInclude(mt => mt.Material)
                            .ThenInclude(m => m.Category));
            if (task is null)
                return (false, $"Task with ID '{taskId}' not found or has been deleted.", default!);

            return (true, "Task retrieved successfully.", task);
        }

        public async Task<(bool IsSuccess, string Message, TaskDto? Task)> GetEmployeeCurrentTaskAsync(Guid employeeId)
        {
            Employee? employee = await _unitOfWork.GetReadRepository<Employee>().GetAsync(
                enableTracking: false,
                includeDeleted: true,
                predicate: e => e.Id == employeeId,
                include: q => q.Include(e => e.CurrentTask)
                               .ThenInclude(t => t!.MaterialTasks)
                               .ThenInclude(mt => mt.Material)
                               .ThenInclude(m => m.Category));
            if (employee is null)
                return (false, "No employee found with the given ID. Please check the ID and try again.", default!);
            if (employee.IsDeleted)
                return (false, $"The employee '{employee.FirstName} {employee.LastName}' is no longer active in the system.", default!);
            if (employee.CurrentTask is null)
                return (false, $"The employee '{employee.FirstName} {employee.LastName}' is currently not assigned to any task.", default!);

            TaskDto taskDto = _mapper.Map<TaskDto>(employee.CurrentTask);
            taskDto.TotalCost = employee.CurrentTask.MaterialTasks
                .Where(mt => mt.Material is not null)
                .Sum(mt => mt.Quantity * mt.Material.Price);

            return (true, $"The employee '{employee.FirstName} {employee.LastName}' is currently working on the task '{taskDto.Title}'.", taskDto);
        }

        /* GetAllActiveTaskListAsync method can use for only and only active list. */
        public async Task<(bool IsSuccess, string Message, IList<TaskDto>? Tasks)> GetAllActiveTaskListAsync()
        {
            IList<Task> tasks = await _unitOfWork.GetReadRepository<Task>().GetAllAsync(
                    enableTracking: true,
                    includeDeleted: false,
                    include: q => q.Include(t => t.Employees)
                                   .Include(t => t.MaterialTasks)
                                   .ThenInclude(mt => mt.Material)
                                   .ThenInclude(m => m.Category),
                    orderBy: q => q.OrderByDescending(t => t.InsertedDate));
            if (!tasks.Any())
                return (false, "No active tasks found. Please ensure there are tasks that are not marked as deleted.", default!);

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

            return (true, "Active tasks have been successfully retrieved and processed.", taskDtos);
        }

        public async Task<(bool IsSuccess, string Message, IList<TaskStatusCountsDto>? TaskCount)> GetListOfTaskCountByStatusAsync()
        {
            IList<Task> taskCounts = await _unitOfWork.GetReadRepository<Task>()
                .GetAllAsync(
                    enableTracking: false,
                    includeDeleted: false);
            if (!taskCounts.Any())
                return (false, "No active tasks found. Please ensure there are tasks with active statuses.", default!);

            List<TaskStatusCountsDto> statusCounts = taskCounts
                .GroupBy(t => t.Status)
                .Select(group => new TaskStatusCountsDto
                {
                    Status = (TaskStatus)group.Key,
                    Count = group.Count()
                }).ToList();
            return (true, "Tasks' counts grouped by status have been successfully retrieved.", statusCounts);
        }
    }
}
