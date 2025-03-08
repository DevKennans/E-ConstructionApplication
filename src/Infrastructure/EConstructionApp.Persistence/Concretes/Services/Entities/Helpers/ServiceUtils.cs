using EConstructionApp.Application.DTOs.Employees;
using EConstructionApp.Application.DTOs.Materials;
using EConstructionApp.Domain.Entities;
using Task = EConstructionApp.Domain.Entities.Task;

namespace EConstructionApp.Persistence.Concretes.Services.Entities.Helpers
{
    public static class ServiceUtils
    {
        #region General
        public static (bool IsValid, string? ErrorMessage) ValidatePagination(int pages, int sizes)
        {
            if (pages < 1 || sizes < 1)
                return (false, "Page number and page size must be greater than zero.");

            return (true, null);
        }
        #endregion

        #region Category
        private const int MinCategoryNameLength = 2;
        private const int MaxCategoryNameLength = 250;

        public static string ValidateCategoryNameForInsert(string? name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return "Category name cannot be empty.";

            name = name.Trim();

            if (name.Length < MinCategoryNameLength)
                return $"Category name must be at least {MinCategoryNameLength} characters long.";
            if (name.Length > MaxCategoryNameLength)
                return $"Category name cannot exceed {MaxCategoryNameLength} characters.";

            return string.Empty;
        }

        public static string ValidateCategoryNameForUpdate(string? name)
        {
            return ValidateCategoryNameForInsert(name);
        }

        public static string GenerateCategorySafeDeleteMessage(Category category, int materialCount)
        {
            return materialCount switch
            {
                0 => $"Category '{category.Name}' has been safely deleted.",
                1 => $"Category '{category.Name}' and 1 associated material have been safely deleted.",
                _ => $"Category '{category.Name}' and {materialCount} associated materials have been safely deleted."
            };
        }

        public static string GenerateCategoryRestoreMessage(Category category, int materialCount)
        {
            return materialCount switch
            {
                0 => $"Category '{category.Name}' has been restored.",
                1 => $"Category '{category.Name}' and 1 associated material have been restored.",
                _ => $"Category '{category.Name}' and {materialCount} associated materials have been restored."
            };
        }
        #endregion

        #region Material
        public static bool HasMaterialChanges(Material material, MaterialUpdateDto materialUpdateDto)
        {
            return material.Name.Trim() != materialUpdateDto.Name.Trim() ||
                   material.Price != materialUpdateDto.Price ||
                   material.StockQuantity != materialUpdateDto.StockQuantity ||
                   material.Measure != materialUpdateDto.Measure ||
                   material.CategoryId != materialUpdateDto.CategoryId;
        }

        public static (bool IsSuccess, string Message) CheckForNoMaterialChanges(Material material, MaterialUpdateDto materialUpdateDto)
        {
            if (!HasMaterialChanges(material, materialUpdateDto))
                return (true, "No changes detected. The update was skipped.");

            return (false, string.Empty);
        }
        #endregion

        #region Employee
        public static bool HasEmployeeChanges(Employee employee, EmployeeUpdateDto employeeUpdateDto)
        {
            return employee.FirstName.Trim() != employeeUpdateDto.FirstName.Trim() ||
                   employee.LastName.Trim() != employeeUpdateDto.LastName.Trim() ||
                   employee.DateOfBirth != employeeUpdateDto.DateOfBirth ||
                   employee.PhoneNumber.Trim() != employeeUpdateDto.PhoneNumber.Trim() ||
                   employee.Address.Trim() != employeeUpdateDto.Address.Trim() ||
                   employee.Salary != employeeUpdateDto.Salary ||
                   employee.Role != employeeUpdateDto.Role;
        }

        public static (bool IsSuccess, string Message) CheckForNoEmployeeChanges(Employee employee, EmployeeUpdateDto employeeUpdateDto)
        {
            if (!HasEmployeeChanges(employee, employeeUpdateDto))
                return (true, "No changes detected. The update was skipped.");

            return (false, string.Empty);
        }
        #endregion

        #region Task
        public static string GenerateInvalidQuantityMessage(string materialName)
        {
            return $"The quantity for material '{materialName}' is invalid. Please specify a quantity greater than zero.";
        }

        public static string GenerateInsufficientStockMessage(string materialName, decimal availableStock, decimal requestedQuantity)
        {
            return $"Insufficient stock. Material '{materialName}' has {availableStock} available, but {requestedQuantity} was requested.";
        }

        public static string GenerateTaskCreationSuccessMessage(string taskTitle, int employeeCount, int materialCount)
        {
            string message = $"Task '{taskTitle}' has been successfully created.";
            List<string> details = new List<string>();

            if (employeeCount == 1)
                details.Add("1 employee has been assigned");
            else if (employeeCount > 1)
                details.Add($"{employeeCount} employees have been assigned");

            if (materialCount == 1)
                details.Add("1 material has been assigned");
            else if (materialCount > 1)
                details.Add($"{materialCount} materials have been assigned");

            if (details.Any())
                message += $" ({string.Join(", ", details)}).";

            return message;
        }

        public static string GenerateTaskEmployeeUpdateMessage(int addedCount, int removedCount)
        {
            string addedMsg = addedCount > 0 ? $"{addedCount} employee{(addedCount > 1 ? "s" : "")} added" : "";
            string removedMsg = removedCount > 0 ? $"{removedCount} employee{(removedCount > 1 ? "s" : "")} removed" : "";

            string resultMessage = $"{(addedMsg + (addedMsg != "" && removedMsg != "" ? ", " : "") + removedMsg).Trim()}.";

            return $"Task employees updated successfully. {resultMessage}";
        }

        public static string GenerateTaskMaterialUpdateMessage(int addedCount, int removedCount, int updatedCount)
        {
            List<string> messages = new List<string>();

            if (addedCount > 0)
                messages.Add($"{addedCount} material{(addedCount > 1 ? "s" : "")} added");
            if (removedCount > 0)
                messages.Add($"{removedCount} material{(removedCount > 1 ? "s" : "")} removed");
            if (updatedCount > 0)
                messages.Add($"{updatedCount} material{(updatedCount > 1 ? "s" : "")} updated");

            string resultMessage = string.Join(", ", messages);
            return resultMessage.Length > 0 ? $"Task materials updated successfully. {resultMessage}." : "No changes detected. Task materials remain the same.";
        }

        public static decimal CalculateTotalTaskCost(Task task)
        {
            return task.MaterialTasks
                .Where(mt => mt.Material is not null)
                .Sum(mt => mt.Quantity * mt.Material.Price);
        }
        #endregion
    }
}
