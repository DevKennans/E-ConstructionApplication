using EConstructionApp.Application.DTOs.Materials;
using EConstructionApp.Application.DTOs.Tasks;
using EConstructionApp.Domain.Entities;

namespace EConstructionApp.Persistence.Concretes.Services.Entities.Helpers
{
    public static class ServiceUtils
    {
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
        public static bool HasMaterialChanges(Material material, MaterialUpdateDto dto)
        {
            return material.Name.Trim() != dto.Name.Trim() ||
                   material.Price != dto.Price ||
                   material.StockQuantity != dto.StockQuantity ||
                   material.CategoryId != dto.CategoryId;
        }

        public static (bool IsSuccess, string Message) CheckForNoMaterialChanges(Material material, MaterialUpdateDto dto)
        {
            if (!HasMaterialChanges(material, dto))
                return (true, "No changes detected. The update was skipped.");

            return (false, string.Empty);
        }
        #endregion

        public static (bool IsSuccess, string Message) ValidateTaskCreationDto(TaskInsertDto taskInsertDto)
        {
            if (taskInsertDto is null)
                return (false, "Task details are missing. Please provide valid information.");

            List<string> errors = new List<string>();

            if (string.IsNullOrWhiteSpace(taskInsertDto.AssignedBy))
                errors.Add("The person assigning the task must be specified.");
            if (string.IsNullOrWhiteSpace(taskInsertDto.AssignedByPhone))
                errors.Add("A contact phone number for the assigner is required.");
            if (string.IsNullOrWhiteSpace(taskInsertDto.AssignedByEmail))
                errors.Add("An email address for the assigner must be provided.");
            if (string.IsNullOrWhiteSpace(taskInsertDto.AssignedByAddress))
                errors.Add("The assigner's address cannot be empty.");

            if (string.IsNullOrWhiteSpace(taskInsertDto.Title))
                errors.Add("Please enter a title for the task.");
            if (string.IsNullOrWhiteSpace(taskInsertDto.Description))
                errors.Add("A task description is required to proceed.");

            if (taskInsertDto.Deadline < DateOnly.FromDateTime(DateTime.Now))
                errors.Add("The deadline must be a future date. Please choose a valid due date.");

            return errors.Any() ? (false, string.Join(" ", errors)) : (true, default!);
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

        public static (bool IsSuccess, string Message) ValidateTaskUpdateDto(TaskDetailsUpdateDto? taskDetailsUpdateDto)
        {
            if (taskDetailsUpdateDto is null)
                return (false, "Task details are missing. Please provide valid information.");

            if (taskDetailsUpdateDto.Deadline < DateOnly.FromDateTime(DateTime.Now))
                return (false, "The deadline must be a future date. Please choose a valid due date.");

            return (true, default!);
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
    }
}
