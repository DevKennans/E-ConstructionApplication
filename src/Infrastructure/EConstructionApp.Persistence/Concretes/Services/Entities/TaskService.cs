﻿using AutoMapper;
using EConstructionApp.Application.DTOs.Tasks;
using EConstructionApp.Application.DTOs.Tasks.Relations;
using EConstructionApp.Application.Interfaces.Services.Entities;
using EConstructionApp.Application.Interfaces.UnitOfWorks;
using EConstructionApp.Application.Validations.Entities.Tasks;
using EConstructionApp.Domain.Entities;
using EConstructionApp.Domain.Entities.Cross;
using EConstructionApp.Domain.Entities.Identification;
using EConstructionApp.Domain.Entities.Relations;
using EConstructionApp.Domain.Enums.Materials;
using EConstructionApp.Persistence.Concretes.Services.Entities.Helpers;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Task = EConstructionApp.Domain.Entities.Task;

namespace EConstructionApp.Persistence.Concretes.Services.Entities
{
    public class TaskService : ITaskService
    {
        private readonly IEmployeeService _employeeService;
        private readonly UserManager<AppUser> _userManager;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public TaskService(IEmployeeService employeeService, UserManager<AppUser> userManager, IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            _employeeService = employeeService;
            _userManager = userManager;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<(bool IsSuccess, string Message)> InsertAsync(TaskInsertDto? taskInsertDto)
        {
            if (taskInsertDto is null)
                return (false, "Invalid task data. Please ensure all required fields are provided.");

            TaskInsertDtoValidator validator = new TaskInsertDtoValidator();
            FluentValidation.Results.ValidationResult validationResult = await validator.ValidateAsync(taskInsertDto);
            if (!validationResult.IsValid)
                return (false, string.Join(" ", validationResult.Errors.Select(e => e.ErrorMessage)));

            Task task = _mapper.Map<Task>(taskInsertDto);

            (bool IsSuccess, string Message, IList<Employee>? Employees, int AssignedCount) employeeResult =
                await AssignEmployeesAsync(taskInsertDto!.EmployeeIds!);
            if (!employeeResult.IsSuccess)
                return (false, employeeResult.Message);

            task.Employees = employeeResult.Employees!;

            (bool IsSuccess, string Message, IList<MaterialTask>? MaterialTasks, int AssignedCount) materialResult =
                await AssignMaterialsAsync(taskInsertDto.MaterialAssignments!);
            if (!materialResult.IsSuccess)
                return (false, materialResult.Message);

            task.MaterialTasks = materialResult.MaterialTasks!;

            await _unitOfWork.GetWriteRepository<Task>().AddAsync(task);
            await _unitOfWork.SaveAsync();

            List<MaterialTransactionLog> materialTransactionLogs = new List<MaterialTransactionLog>();
            if (task.MaterialTasks != null)
            {
                foreach (MaterialTask mt in task.MaterialTasks)
                {
                    var material = await _unitOfWork.GetReadRepository<Material>()
                        .GetAsync(m => m.Id == mt.MaterialId, enableTracking: false);

                    if (material != null && material.Price > 0)
                    {
                        decimal totalCost = mt.Quantity * material.Price;
                        materialTransactionLogs.Add(new MaterialTransactionLog
                        {
                            TaskId = task.Id,
                            MaterialId = mt.MaterialId,
                            Quantity = mt.Quantity,
                            Measure = material.Measure,
                            PriceAtTransaction = totalCost,
                            TransactionType = MaterialTransactionType.Added
                        });
                    }
                }
            }

            if (materialTransactionLogs.Any())
            {
                await _unitOfWork.GetWriteRepository<MaterialTransactionLog>().AddRangeAsync(materialTransactionLogs);
                await _unitOfWork.SaveAsync();
            }

            return (true,
                ServiceUtils.GenerateTaskCreationSuccessMessage(task.Title, employeeResult.AssignedCount,
                    materialResult.AssignedCount));
        }

        private async Task<(bool IsSuccess, string Message, IList<Employee>? Employees, int AssignedCount)>
            AssignEmployeesAsync(IList<Guid>? employeeIds)
        {
            if (employeeIds is null || !employeeIds.Any())
                return (true, "No employees have been assigned to the task.", null, default!);

            (bool IsSuccess, string Message, IList<Application.DTOs.Employees.EmployeeDto>? AvailableEmployees) =
                await _employeeService.GetAvailableEmployeesListAsync();
            if (!IsSuccess)
                return (false, "There are no available employees to assign at the moment.", null, default!); /* NOTE
                     * Although it is logically impossible to get an ID like this from the MVC, i.e. the display part, it can be mistaken for the API.
                     */

            List<Application.DTOs.Employees.EmployeeDto> validEmployees =
                AvailableEmployees!.Where(e => employeeIds.Contains(e.Id)).ToList();
            if (validEmployees.Count != employeeIds.Count)
                return (false, "Some of the employees are either unavailable or already assigned to other tasks.", null,
                    default!);

            IList<Employee> employeeEntities = _mapper.Map<IList<Employee>>(validEmployees);
            foreach (Employee employee in employeeEntities)
            {
                employee.IsCurrentlyWorking = true;
                await _unitOfWork.GetWriteRepository<Employee>().UpdateAsync(employee);
            }

            return (true, "Employees have been successfully assigned to the task.", employeeEntities,
                employeeEntities.Count);
        }

        private async Task<(bool IsSuccess, string Message, IList<MaterialTask>? MaterialTasks, int AssignedCount)>
            AssignMaterialsAsync(IList<MaterialAssignmentInsertDto>? materialAssignments)
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
                return (false, "Some materials are either inactive or cannot be assigned at the moment.", null,
                    default!);

            List<string> errorMessages = new List<string>();
            List<MaterialTask> materialTasks = new List<MaterialTask>();

            foreach (MaterialAssignmentInsertDto assignment in groupedAssignments)
            {
                Material? material = validMaterials.FirstOrDefault(m => m.Id == assignment.MaterialId);
                if (material is null)
                    continue;

                if (assignment.Quantity <= 0)
                    errorMessages.Add(ServiceUtils.GenerateInvalidQuantityMessage(material.Name));
                else if (material.StockQuantity < assignment.Quantity)
                    errorMessages.Add(ServiceUtils.GenerateInsufficientStockMessage(material.Name,
                        material.StockQuantity, assignment.Quantity));
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
                return (false, string.Join(" ", errorMessages), null, default!);

            return (true, "Materials have been successfully assigned to the task.", materialTasks, materialTasks.Count);
        }

        public async Task<(bool IsSuccess, string Message)> UpdateTasksDetailsAsync(
            TaskDetailsUpdateDto? taskDetailsUpdateDto)
        {
            if (taskDetailsUpdateDto is null)
                return (false, "Invalid task data. Please ensure all required fields are provided.");

            Task? task = await _unitOfWork.GetReadRepository<Task>().GetAsync(
                enableTracking: true,
                includeDeleted: true,
                predicate: t => t.Id == taskDetailsUpdateDto!.Id,
                include: q => q.Include(t => t.Employees)
                    .Include(t => t.MaterialTasks)
                    .ThenInclude(mt => mt.Material)
                    .ThenInclude(m => m.Category));
            if (task is null)
                return (false, "The selected task could not be found in the system. Please check and try again.");
            if (task.IsDeleted)
                return (false, "The selected task is inactive. Please restore it before proceeding.");

            TaskUpdateDtoValidator validator = new TaskUpdateDtoValidator();
            FluentValidation.Results.ValidationResult validationResult =
                await validator.ValidateAsync(taskDetailsUpdateDto);
            if (!validationResult.IsValid)
                return (false, string.Join(" ", validationResult.Errors.Select(e => e.ErrorMessage)));

            List<string> updates = ApplyTaskUpdates(taskDetailsUpdateDto!, task!);
            if (!updates.Any())
                return (false, "No updates detected. The task details remain unchanged.");

            await _unitOfWork.GetWriteRepository<Task>().UpdateAsync(task!);
            await _unitOfWork.SaveAsync();

            if (taskDetailsUpdateDto.Status == Domain.Enums.Tasks.TaskStatus.Cancelled ||
                taskDetailsUpdateDto.Status == Domain.Enums.Tasks.TaskStatus.Completed)
            {
                await UpdateTasksEmployeesAsync(taskDetailsUpdateDto.Id, new List<Guid>());
                await _unitOfWork.SaveAsync();
            }

            return (true, $"Task '{task!.Title}' has been successfully updated! {string.Join(" ", updates)}");
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

            UpdateField(taskDetailsUpdateDto.AssignedBy, task.AssignedBy, newValue => task.AssignedBy = newValue,
                "AssignedBy");
            UpdateField(taskDetailsUpdateDto.AssignedByPhone, task.AssignedByPhone,
                newValue => task.AssignedByPhone = newValue, "AssignedByPhone");
            UpdateField(taskDetailsUpdateDto.AssignedByAddress, task.AssignedByAddress,
                newValue => task.AssignedByAddress = newValue, "AssignedByAddress");
            UpdateField(taskDetailsUpdateDto.Title, task.Title, newValue => task.Title = newValue, "Title");
            UpdateField(taskDetailsUpdateDto.Description, task.Description, newValue => task.Description = newValue,
                "Description");
            UpdateField(taskDetailsUpdateDto.Deadline, task.Deadline, newValue => task.Deadline = newValue, "Deadline");
            UpdateField(taskDetailsUpdateDto.Priority, task.Priority, newValue => task.Priority = newValue, "Priority");
            UpdateField(taskDetailsUpdateDto.Status, task.Status, newValue => task.Status = newValue, "Status");

            return updates;
        }

        public async Task<(bool IsSuccess, string Message)> UpdateTasksEmployeesAsync(Guid taskId,
            List<Guid>? updatedEmployeeIds)
        {
            (bool IsSuccess, string Message, Task? Task) = await GetTaskWithRelationsByIdAsync(taskId);
            if (!IsSuccess)
                return (false, Message);

            List<Guid> currentEmployeeIds = Task!.Employees.Select(e => e.Id).ToList();
            List<Guid> employeesToRemove = currentEmployeeIds.Except(updatedEmployeeIds!).ToList();
            List<Guid> employeesToAdd = updatedEmployeeIds!.Except(currentEmployeeIds).ToList();

            if (!employeesToRemove.Any() && !employeesToAdd.Any())
                return (true, "No changes detected. Task employees remain the same.");

            if (employeesToAdd.Any() && (Task.Status == Domain.Enums.Tasks.TaskStatus.Cancelled ||
                                         Task.Status == Domain.Enums.Tasks.TaskStatus.Completed))
                return (false, "Cannot add employees to a task that is completed or cancelled.");

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

            foreach (Employee employee in employeesToBeRemoved)
                UpdateEmployeeTaskRelationship(employee, null);
            foreach (Employee employee in employeesToBeAdded)
                UpdateEmployeeTaskRelationship(employee, taskId);

            await _unitOfWork.SaveAsync();

            return (true, ServiceUtils.GenerateTaskEmployeeUpdateMessage(addedCount, removedCount));
        }

        private void UpdateEmployeeTaskRelationship(Employee employee, Guid? taskId)
        {
            employee.CurrentTaskId = taskId;
        }

        public async Task<(bool IsSuccess, string Message)> UpdateTasksMaterialsAsync(Guid taskId,
            List<MaterialAssignmentInsertDto>? updatedMaterials)
        {
            Task? task = await _unitOfWork.GetReadRepository<Task>().GetAsync(
                enableTracking: true,
                includeDeleted: false,
                predicate: t => t.Id == taskId,
                include: q => q.Include(t => t.Employees)
                    .Include(t => t.MaterialTasks)
                    .ThenInclude(mt => mt.Material)
                    .ThenInclude(m => m.Category));
            if (task is null)
                return (false, $"Task with ID '{taskId}' not found or has been deleted.");

            List<MaterialTask> currentMaterials = task.MaterialTasks.ToList();
            List<Guid> currentMaterialIds = currentMaterials.Select(m => m.MaterialId).ToList();
            List<Guid> updatedMaterialIds = updatedMaterials!.Select(m => m.MaterialId).ToList();

            List<Guid> materialsToRemove = currentMaterialIds.Except(updatedMaterialIds).ToList();

            IList<Material> validMaterials = await _unitOfWork.GetReadRepository<Material>().GetAllAsync(
                enableTracking: true,
                includeDeleted: false,
                predicate: m => updatedMaterialIds.Contains(m.Id) || materialsToRemove.Contains(m.Id));
            List<Guid> missingMaterialIds = updatedMaterialIds.Except(validMaterials.Select(m => m.Id)).ToList();
            if (missingMaterialIds.Any())
                return (false,
                    $"One or more materials are invalid or inactive. Missing Material IDs: {string.Join(", ", missingMaterialIds)}");

            List<MaterialAssignmentInsertDto> groupedMaterials = updatedMaterials!
                .GroupBy(m => m.MaterialId)
                .Select(g => new MaterialAssignmentInsertDto
                {
                    MaterialId = g.Key,
                    Quantity = g.Sum(m => m.Quantity)
                }).ToList();

            int removedCount = await RemoveMaterials(task, validMaterials, materialsToRemove, currentMaterials);

            List<MaterialAssignmentInsertDto> materialsToAdd =
                groupedMaterials.Where(m => !currentMaterialIds.Contains(m.MaterialId)).ToList();
            (bool IsAddSuccess, int AddedCount, string AddMessage) =
                await AddMaterials(task, validMaterials, materialsToAdd);
            if (!IsAddSuccess)
                return (false, AddMessage);

            List<MaterialAssignmentInsertDto> materialsToUpdate = groupedMaterials
                .Where(m => currentMaterialIds.Contains(m.MaterialId) &&
                            currentMaterials.FirstOrDefault(cm => cm.MaterialId == m.MaterialId)?.Quantity !=
                            m.Quantity)
                .ToList();

            (bool IsUpdateSuccess, int UpdatedCount, string UpdateMessage) =
                await UpdateMaterials(task, validMaterials, materialsToUpdate, currentMaterials);
            if (!IsUpdateSuccess)
                return (false, UpdateMessage);

            return (true, ServiceUtils.GenerateTaskMaterialUpdateMessage(AddedCount, removedCount, UpdatedCount));
        }

        private async Task<int> RemoveMaterials(Task task, IList<Material> validMaterials, List<Guid> materialsToRemove,
            List<MaterialTask> currentMaterials)
        {
            int removedCount = 0;
            List<MaterialTransactionLog> materialTransactionLogs = new List<MaterialTransactionLog>();

            foreach (Guid materialId in materialsToRemove)
            {
                MaterialTask? materialTask = currentMaterials.FirstOrDefault(m => m.MaterialId == materialId);
                Material? material = validMaterials.FirstOrDefault(m => m.Id == materialId);

                if (materialTask is not null && material is not null)
                {
                    // Restore stock before removing the material
                    RestoreStock(material, materialTask.Quantity);

                    // Remove the material task from the task's material tasks
                    task.MaterialTasks.Remove(materialTask);

                    // Add transaction log after the task removal (log after the operation)
                    materialTransactionLogs.Add(new MaterialTransactionLog
                    {
                        TaskId = task.Id,
                        MaterialId = materialId,
                        Quantity = materialTask.Quantity,
                        Measure = material.Measure,
                        PriceAtTransaction = materialTask.Quantity * material.Price,
                        TransactionType = MaterialTransactionType.Removed
                    });

                    removedCount++;
                }
            }

            // Save all changes after the operation
            await _unitOfWork.SaveAsync();

            // If there are any transaction logs, save them as well
            if (materialTransactionLogs.Any())
            {
                await _unitOfWork.GetWriteRepository<MaterialTransactionLog>().AddRangeAsync(materialTransactionLogs);
                await _unitOfWork.SaveAsync();
            }

            return removedCount;
        }

        private async Task<(bool IsSuccess, int AddedCount, string Message)> AddMaterials(Task task,
            IList<Material> validMaterials, List<MaterialAssignmentInsertDto> materialsToAdd)
        {
            int addedCount = 0;

            foreach (MaterialAssignmentInsertDto materialDto in materialsToAdd)
            {
                Material? material = validMaterials.FirstOrDefault(m => m.Id == materialDto.MaterialId);
                if (material is null || material.IsDeleted)
                    continue;

                if (material.StockQuantity < materialDto.Quantity)
                    return (false, 0,
                        $"Insufficient stock for material '{material.Name}'. Available: {material.StockQuantity}, Required: {materialDto.Quantity}");

                DeductStock(material, materialDto.Quantity);

                MaterialTask materialTask = new MaterialTask
                {
                    MaterialId = materialDto.MaterialId,
                    TaskId = task.Id,
                    Quantity = materialDto.Quantity
                };
                task.MaterialTasks.Add(materialTask);

                addedCount++;
            }

            await _unitOfWork.SaveAsync();

            List<MaterialTransactionLog> materialTransactionLogs = new List<MaterialTransactionLog>();
            foreach (MaterialAssignmentInsertDto material in materialsToAdd)
            {
                if (material is not null)
                {
                    Material dbMaterial = await _unitOfWork.GetReadRepository<Material>()
                        .GetAsync(predicate: m => m.Id == material.MaterialId);
                    decimal totalCost = material.Quantity * dbMaterial.Price;

                    materialTransactionLogs.Add(new MaterialTransactionLog()
                    {
                        TaskId = task.Id,
                        MaterialId = material.MaterialId,
                        Quantity = material.Quantity,
                        Measure = dbMaterial.Measure,
                        PriceAtTransaction = totalCost,
                        TransactionType = MaterialTransactionType.Added
                    });
                }
            }

            if (materialTransactionLogs.Any())
            {
                await _unitOfWork.GetWriteRepository<MaterialTransactionLog>().AddRangeAsync(materialTransactionLogs);
                await _unitOfWork.SaveAsync();
            }

            return (true, addedCount, string.Empty);
        }

        private async Task<(bool IsSuccess, int UpdatedCount, string Message)> UpdateMaterials(Task task,
            IList<Material> validMaterials, List<MaterialAssignmentInsertDto> materialsToUpdate,
            List<MaterialTask> currentMaterials)
        {
            int updatedCount = 0;
            List<MaterialTransactionLog> materialTransactionLogs = new List<MaterialTransactionLog>();

            foreach (MaterialAssignmentInsertDto materialDto in materialsToUpdate)
            {
                MaterialTask? existingTaskMaterial =
                    currentMaterials.FirstOrDefault(m => m.MaterialId == materialDto.MaterialId);
                Material? material = validMaterials.FirstOrDefault(m => m.Id == materialDto.MaterialId);

                if (existingTaskMaterial is not null && material is not null)
                {
                    int quantityDifference = (int)Math.Round(materialDto.Quantity - existingTaskMaterial.Quantity);

                    if (quantityDifference > 0)
                    {
                        // If quantity increased, check stock availability and deduct it
                        if (material.StockQuantity < quantityDifference)
                        {
                            return (false, 0,
                                $"Insufficient stock for material '{material.Name}'. Available: {material.StockQuantity}, Required additional: {quantityDifference}");
                        }

                        // Deduct stock for the added quantity
                        DeductStock(material, quantityDifference);

                        // Log the addition transaction
                        materialTransactionLogs.Add(new MaterialTransactionLog
                        {
                            TaskId = task.Id,
                            MaterialId = materialDto.MaterialId,
                            Quantity = quantityDifference,
                            Measure = material.Measure,
                            PriceAtTransaction = quantityDifference * material.Price,
                            TransactionType = MaterialTransactionType.Added
                        });
                    }
                    else if (quantityDifference < 0)
                    {
                        // If quantity decreased, check if there is enough stock to restore
                        RestoreStock(material, -quantityDifference);

                        // Log the removal transaction
                        materialTransactionLogs.Add(new MaterialTransactionLog
                        {
                            TaskId = task.Id,
                            MaterialId = materialDto.MaterialId,
                            Quantity = -quantityDifference,
                            Measure = material.Measure,
                            PriceAtTransaction = -quantityDifference * material.Price,
                            TransactionType = MaterialTransactionType.Removed
                        });
                    }

                    // Update the material task quantity
                    existingTaskMaterial.Quantity = materialDto.Quantity;
                    updatedCount++;
                }
            }

            // Save task and transaction logs after the operation
            await _unitOfWork.SaveAsync();

            if (materialTransactionLogs.Any())
            {
                await _unitOfWork.GetWriteRepository<MaterialTransactionLog>().AddRangeAsync(materialTransactionLogs);
                await _unitOfWork.SaveAsync();
            }

            return (true, updatedCount, string.Empty);
        }

        private void RestoreStock(Material material, decimal quantity)
        {
            material.StockQuantity += quantity;
        }

        private void DeductStock(Material material, decimal quantity)
        {
            material.StockQuantity -= quantity;
        }

        private void AdjustStock(Material material, decimal quantityDifference)
        {
            material.StockQuantity -= quantityDifference;
        }

        public async Task<(bool IsSuccess, string Message, int ActiveTasks, int TotalTasks)>
            GetBothActiveAndTotalCountsAsync()
        {
            int totalTasks = await _unitOfWork.GetReadRepository<Task>().CountAsync(includeDeleted: true);
            if (totalTasks == 0)
                return (false, "No materials exist in the system.", default!, default!);

            int activeTasks = await _unitOfWork.GetReadRepository<Task>().CountAsync(
                includeDeleted: false);

            return (true, "Tasks' counts have been successfully retrieved.", activeTasks, totalTasks);
        }

        private async Task<(bool IsSuccess, string Message, Task? Task)> GetTaskWithRelationsByIdAsync(Guid taskId)
        {
            Task? task = await _unitOfWork.GetReadRepository<Task>().GetAsync(
                enableTracking: false,
                includeDeleted: false,
                predicate: t => t.Id == taskId,
                include: q => q.Include(t => t.Employees)
                    .Include(t => t.MaterialTasks)
                    .ThenInclude(mt => mt.Material)
                    .ThenInclude(m => m.Category));
            if (task is null)
                return (false, $"Task with ID '{taskId}' not found or has been deleted.", null);

            return (true, "Task retrieved successfully.", task);
        }

        public async Task<(bool IsSuccess, string Message, TaskDto? Task)>
            GetEmployeeCurrentTaskAsync(Guid employeeId) /*
                                                          * Type of this employee is AppUser, but in according service it will be checked for another kind of users like admin & moderator.
                                                          * EmployeeId from AppUser must be bind with Employee data!
                                                          */
        {
            AppUser? appUserEmployee = await _userManager.Users
                .FirstOrDefaultAsync(user => user.Id == employeeId.ToString());
            if (appUserEmployee is null)
                return (false, "No employee found with the provided ID.", null!);

            bool hasEmployeePermission = await _userManager.IsInRoleAsync(appUserEmployee, "Employee");
            if (!hasEmployeePermission)
                return (false,
                    "The selected user does not have permission to access employee's task data. (Check user type again, user must have a 'employee' role for doing operation)",
                    null!);

            Employee? employee = await _unitOfWork.GetReadRepository<Employee>().GetAsync(
                enableTracking: false,
                includeDeleted: true,
                predicate: emp => emp.PhoneNumber == appUserEmployee.PhoneNumber,
                include: q => q.Include(e => e.CurrentTask)
                    .ThenInclude(t => t!.MaterialTasks)
                    .ThenInclude(mt => mt.Material)
                    .ThenInclude(m => m.Category));
            if (employee is null)
                return (false, "No employee found with the given ID. Please verify and try again.", null);
            if (employee.IsDeleted)
                return (false, $"The employee '{employee.FirstName} {employee.LastName}' is no longer active.", null);
            if (employee.CurrentTask is null)
                return (false,
                    $"The employee '{employee.FirstName} {employee.LastName}' is not currently assigned to any task.",
                    null);

            TaskDto taskDto = _mapper.Map<TaskDto>(employee.CurrentTask);
            taskDto.TotalCost = ServiceUtils.CalculateTotalTaskCost(employee.CurrentTask);

            return (true,
                $"The employee '{employee.FirstName} {employee.LastName}' is currently working on the task '{taskDto.Title}'.",
                taskDto);
        }

        public async Task<(bool IsSuccess, string Message, IList<TaskDto>? Tasks)> GetAllActiveTasksListAsync()
        {
            IList<Task> tasks = await _unitOfWork.GetReadRepository<Task>().GetAllAsync(
                enableTracking: true, /* need to care about tracking tasks data! */
                includeDeleted: false,
                include: q => q.Include(t => t.Employees)
                    .Include(t => t.MaterialTasks)
                    .ThenInclude(mt => mt.Material)
                    .ThenInclude(m => m.Category),
                orderBy: q => q.OrderByDescending(t => t.InsertedDate));
            if (!tasks.Any())
                return (false, "No active tasks available. Please check if there are any active tasks.", null);

            IList<TaskDto> taskDtos = _mapper.Map<IList<TaskDto>>(tasks);
            foreach (TaskDto taskDto in taskDtos)
            {
                Task? correspondingTask = tasks.FirstOrDefault(t => t.Id == taskDto.Id);
                if (correspondingTask is not null)
                    taskDto.TotalCost = ServiceUtils.CalculateTotalTaskCost(correspondingTask);
            }

            return (true, "Active tasks have been successfully retrieved.", taskDtos);
        }

        public async Task<(bool IsSuccess, string Message, IList<TaskDto>? Tasks, int TotalTasks)>
            GetOnlyActiveTasksPagedListAsync(int pages = 1, int sizes = 5)
        {
            (bool IsValid, string? ErrorMessage) validation = ServiceUtils.ValidatePagination(pages, sizes);
            if (!validation.IsValid)
                return (false, validation.ErrorMessage!, null, default!);

            IList<Task> tasks = await _unitOfWork.GetReadRepository<Task>().GetAllByPagingAsync(
                enableTracking: true, /* need to care about tracking tasks data! */
                includeDeleted: false,
                include: q => q.Include(t => t.Employees)
                    .Include(t => t.MaterialTasks)
                    .ThenInclude(mt => mt.Material)
                    .ThenInclude(m => m.Category),
                currentPage: pages,
                pageSize: sizes,
                orderBy: q => q.OrderByDescending(t => t.InsertedDate));

            int totalTasks = await _unitOfWork.GetReadRepository<Task>().CountAsync(includeDeleted: false);

            if (!tasks.Any())
                return (false, "No active tasks available. Please check if there are any active tasks.", null,
                    totalTasks);

            IList<TaskDto> taskDtos = _mapper.Map<IList<TaskDto>>(tasks);
            foreach (TaskDto taskDto in taskDtos)
            {
                Task? correspondingTask = tasks.FirstOrDefault(t => t.Id == taskDto.Id);
                if (correspondingTask is not null)
                    taskDto.TotalCost = ServiceUtils.CalculateTotalTaskCost(correspondingTask);
            }

            return (true, "Active tasks have been successfully retrieved.", taskDtos, totalTasks);
        }

        public async Task<(bool IsSuccess, string Message, IList<TaskStatusCountsDto>? TaskCount)>
            GetListOfTasksCountsByStatusAsync()
        {
            IList<Task> taskCounts = await _unitOfWork.GetReadRepository<Task>()
                .GetAllAsync(
                    enableTracking: false,
                    includeDeleted: false);
            if (!taskCounts.Any())
                return (false, "No active tasks available. Please check if any tasks exist with an active status.",
                    null);

            List<TaskStatusCountsDto> statusCounts = taskCounts
                .GroupBy(t => t.Status)
                .Select(group => new TaskStatusCountsDto
                {
                    Status = (TaskStatus)group.Key,
                    Count = group.Count()
                }).ToList();
            return (true, "Tasks' counts have been successfully grouped by status.", statusCounts);
        }

        public async System.Threading.Tasks.Task FinishCurrentTaskById(Guid taskId)
        {
            Task? task = await _unitOfWork.GetReadRepository<Task>().GetAsync(
                enableTracking: true,
                includeDeleted: false,
                predicate: t => t.Id == taskId,
                include: q => q.Include(t => t.Employees)
                    .Include(t => t.MaterialTasks)
                    .ThenInclude(mt => mt.Material)
                    .ThenInclude(m => m.Category));

            task.Status = Domain.Enums.Tasks.TaskStatus.Completed;
            await _unitOfWork.SaveAsync();

            await UpdateTasksEmployeesAsync(taskId, new List<Guid>());
            await _unitOfWork.SaveAsync();
        }

        public async Task<(bool IsSuccess, string Message, Domain.Entities.Task? Task)> GetTaskByIdAsync(Guid taskId)
        {
            var task = await _unitOfWork.GetReadRepository<Domain.Entities.Task>()
                .GetAsync(
                    predicate: t => t.Id == taskId,
                    include: t => t.Include(x => x.Employees)
                );

            if (task is null)
                return (false, "Task not found.", null);

            return (true, "Task retrieved successfully.", task);
        }

        public async Task<(bool IsSuccess, string Message, List<Guid>? EmployeeIds)> GetTaskEmployeeIdsAsync(Guid taskId)
        {
            var task = await _unitOfWork.GetReadRepository<Domain.Entities.Task>()
                .GetAsync(
                    predicate: t => t.Id == taskId,
                    include: t => t.Include(x => x.Employees)
                );

            if (task is null)
                return (false, "Task not found.", null);

            var employeeIds = task.Employees.Select(e => e.Id).ToList();

            return (true, "Employee IDs retrieved successfully.", employeeIds);
        }

    }
}