using EConstructionApp.Application.DTOs.Employees;
using EConstructionApp.Application.DTOs.Materials;
using EConstructionApp.Application.DTOs.Tasks;
using EConstructionApp.Application.DTOs.Tasks.Relations;
using EConstructionApp.Application.Interfaces.Services.Entities;
using EConstructionApp.Domain.Enums.Tasks;
using EConstructionApp.WebUI.Areas.Admin.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using FirebaseAdmin.Messaging;
using TaskStatus = EConstructionApp.Domain.Enums.Tasks.TaskStatus;
using System.Diagnostics;
using EConstructionApp.Application.Interfaces.Services.Identification;

namespace EConstructionApp.WebUI.Areas.Admin.Controllers
{
    //[Authorize]
    [Area("Admin")]
    public class TaskController : Controller
    {
        private readonly ITaskService _taskService;
        private readonly IEmployeeService _employeeService;
        private readonly IMaterialService _materialService;
        private readonly IAuthService _authService;
        public TaskController(ITaskService taskService, IEmployeeService employeeService, IMaterialService materialService, IAuthService authService)
        {
            _taskService = taskService;
            _employeeService = employeeService;
            _materialService = materialService;
            _authService = authService;
        }

        public async Task<IActionResult> CreateTask()
        {
            
            var employeeResult = await _employeeService.GetAvailableEmployeesListAsync();
            var materialResult = await _materialService.GetAvailableMaterialsListAsync();

            var model = new TaskCreateViewModel
            {
                Employees = employeeResult.Employees ?? new List<EmployeeDto>(),
                Materials = materialResult.Materials ?? new List<MaterialDto>(),
                Priorities = Enum.GetValues(typeof(TaskPriority))
                        .Cast<TaskPriority>()
                        .Select(e => new SelectListItem
                        {
                            Value = e.ToString(),
                            Text = e.ToString()
                        }).ToList()
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> CreateTask(TaskCreateViewModel model)
        {
            var (isSuccess, message) = await _taskService.InsertAsync(model.Task);
            var employeeResult = await _employeeService.GetAvailableEmployeesListAsync();
            var materialResult = await _materialService.GetAvailableMaterialsListAsync();

            model.Employees = employeeResult.Employees;
            model.Materials = materialResult.Materials;
            model.Priorities = Enum.GetValues(typeof(TaskPriority))
                        .Cast<TaskPriority>()
                        .Select(e => new SelectListItem
                        {
                            Value = e.ToString(),
                            Text = e.ToString()
                        }).ToList();
            if (isSuccess)
            {
                if (model.Task.EmployeeIds != null && model.Task.EmployeeIds.Any())
                {
                    var deviceTokens = await _authService.GetFcmTokensByEmployeeIdsAsync(model.Task.EmployeeIds);
                    Debug.WriteLine($"Device tokens: {string.Join(", ", deviceTokens)}");
                    foreach (var token in deviceTokens)
                    {
                        if (string.IsNullOrWhiteSpace(token))
                        {
                            Debug.WriteLine("Token is null or empty!");
                            continue;
                        }

                        var notfMessage = new Message()
                        {
                            Notification = new Notification
                            {
                                Title = model.Task.Title ?? "Yeni Tapşırıq",
                                Body = model.Task.Description ?? "Tapşırıq təfərrüatları üçün tətbiqə baxın"
                            },
                            Data = new Dictionary<string, string>
                                {
                                    { "taskBy", model.Task.AssignedBy }
                                },
                            Token = token,
                        };

                        try
                        {
                            string response = await FirebaseMessaging.DefaultInstance.SendAsync(notfMessage);
                            Debug.WriteLine($"Notification sent. Response: {response}");
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine($"Error sending notification: {ex.Message}");
                        }
                    }

                }
                TempData["SuccessMessageFromTask"] = message;
                model.Task = new TaskInsertDto();
                return View(model);
            }

            TempData["ErrorMessageFromTask"] = message;
            return View(model);
        }

        public async Task<IActionResult> GetTasks(int page = 1, int size = 6)
        {
            var result = await _taskService.GetOnlyActiveTasksPagedListAsync(page, size);
            var res = await _employeeService.GetAvailableEmployeesListAsync();
            var r = await _materialService.GetAvailableMaterialsListAsync();

            var totalPages = (int)Math.Ceiling((double)result.TotalTasks / size);

            var model = new TaskViewModel
            {
                Tasks = result.IsSuccess ? result.Tasks : new List<TaskDto>(),
                Employees = result.IsSuccess ? res.Employees : new List<EmployeeDto>(),
                Materials = result.IsSuccess ? r.Materials : new List<MaterialDto>(),
                Priorities = Enum.GetValues(typeof(TaskPriority))
                                 .Cast<TaskPriority>()
                                 .Select(e => new SelectListItem
                                 {
                                     Value = e.ToString(),
                                     Text = e.ToString()
                                 }).ToList(),
                Statuses = Enum.GetValues(typeof(TaskStatus))
                               .Cast<TaskStatus>()
                               .Select(e => new SelectListItem
                               {
                                   Value = e.ToString(),
                                   Text = e.ToString()
                               }).ToList(),
                CurrentPage = page,
                TotalPages = totalPages
            };

            if (!result.IsSuccess)
                TempData["ErrorMessageFromTask"] = result.Message;

            return View(model);
        }




        [HttpPost]
        public async Task<IActionResult> EditTask(TaskDetailsUpdateDto viewModel)
        {
            var (isSuccess, message) = await _taskService.UpdateTasksDetailsAsync(viewModel);
            if (!isSuccess)
            {
                TempData["ErrorMessageFromTask"] = message;
            }
            else
            {
                TempData["SuccessMessageFromTask"] = message;
            }
            return RedirectToAction("GetTasks");
        }

        [HttpPost]
        public async Task<IActionResult> UpdateTaskEmployees(Guid taskId, string? updatedEmployeeIds)
        {
            var employeeIdList = updatedEmployeeIds?.Split(',')
                                           .Select(id => Guid.TryParse(id, out var guid) ? guid : (Guid?)null)
                                           .Where(guid => guid.HasValue)
                                           .Select(guid => guid.Value)
                                           .ToList() ?? new List<Guid>();

            // Əvvəlki işçiləri al
            var (prevSuccess, _, previousEmployeeIds) = await _taskService.GetTaskEmployeeIdsAsync(taskId);

            // Dəyişiklikləri yadda saxla
            var (isSuccess, message) = await _taskService.UpdateTasksEmployeesAsync(taskId, employeeIdList);

            if (isSuccess)
            {
                if (prevSuccess && previousEmployeeIds is not null)
                {
                    // ✅ Əlavə olunanlar
                    var addedEmployeeIds = employeeIdList.Except(previousEmployeeIds).ToList();

                    if (addedEmployeeIds.Any())
                    {
                        var deviceTokens = await _authService.GetFcmTokensByEmployeeIdsAsync(addedEmployeeIds);
                        foreach (var token in deviceTokens)
                        {
                            if (string.IsNullOrWhiteSpace(token))
                                continue;

                            var notfMessage = new Message()
                            {
                                Notification = new Notification
                                {
                                    Title = "Yeni tapşırıq",
                                    Body = "Sizə yeni tapşırıq təyin olunub. Ətraflı məlumat üçün tətbiqə baxın."
                                },
                                Data = new Dictionary<string, string>
                        {
                            { "taskId", taskId.ToString() }
                        },
                                Token = token,
                            };

                            try
                            {
                                string response = await FirebaseMessaging.DefaultInstance.SendAsync(notfMessage);
                                Debug.WriteLine($"Notification sent to new employee. Response: {response}");
                            }
                            catch (Exception ex)
                            {
                                Debug.WriteLine($"Error sending notification to new employee: {ex.Message}");
                            }
                        }
                    }

                    var removedEmployeeIds = previousEmployeeIds.Except(employeeIdList).ToList();

                    if (removedEmployeeIds.Any())
                    {
                        var deviceTokens = await _authService.GetFcmTokensByEmployeeIdsAsync(removedEmployeeIds);
                        foreach (var token in deviceTokens)
                        {
                            if (string.IsNullOrWhiteSpace(token))
                                continue;

                            var notfMessage = new Message()
                            {
                                Notification = new Notification
                                {
                                    Title = "Tapşırıq ləğv edildi",
                                    Body = "Siz bu tapşırıqdan çıxarıldınız."
                                },
                                Data = new Dictionary<string, string>
                        {
                            { "taskId", taskId.ToString() }
                        },
                                Token = token,
                            };

                            try
                            {
                                string response = await FirebaseMessaging.DefaultInstance.SendAsync(notfMessage);
                                Debug.WriteLine($"Notification sent to removed employee. Response: {response}");
                            }
                            catch (Exception ex)
                            {
                                Debug.WriteLine($"Error sending notification to removed employee: {ex.Message}");
                            }
                        }
                    }
                }

                TempData["SuccessMessageFromTask"] = message;
            }
            else
            {
                TempData["ErrorMessageFromTask"] = message;
            }

            return RedirectToAction("GetTasks");
        }




        [HttpPost]
        public async Task<IActionResult> UpdateTaskMaterials(Guid taskId, string updatedMaterialIds)
        {
            List<MaterialAssignmentInsertDto> updatedMaterials;
            try
            {
                updatedMaterials = JsonConvert.DeserializeObject<List<MaterialAssignmentInsertDto>>(updatedMaterialIds);
            }
            catch
            {
                TempData["ErrorMessageFromTask"] = "Invalid material data!";
                return RedirectToAction("GetTasks");
            }
            var (isSuccess, message) = await _taskService.UpdateTasksMaterialsAsync(taskId, updatedMaterials);

            TempData[isSuccess ? "SuccessMessageFromTask" : "ErrorMessageFromTask"] = message;
            return RedirectToAction("GetTasks");
        }
    }
}
