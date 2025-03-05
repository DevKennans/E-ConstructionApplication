using EConstructionApp.Application.DTOs.Tasks;

namespace EConstructionApp.Persistence.Concretes.Services.Entities.Helpers
{
    public static class TaskServiceHelper
    {
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
