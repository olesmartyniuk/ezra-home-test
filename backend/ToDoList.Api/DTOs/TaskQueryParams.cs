using System.ComponentModel.DataAnnotations;
using ToDoList.Api.Domain.Enums;

namespace ToDoList.Api.DTOs;

public class TaskQueryParams
{
    public TaskItemStatus? Status { get; set; }
    public TaskPriority? Priority { get; set; }

    [AllowedValues("createdAt", "dueDate", "priority", "status",
        ErrorMessage = "sortBy must be one of: createdAt, dueDate, priority, status.")]
    public string? SortBy { get; set; } = "createdAt";

    [AllowedValues("asc", "desc",
        ErrorMessage = "sortOrder must be 'asc' or 'desc'.")]
    public string? SortOrder { get; set; } = "desc";

    public string? Search { get; set; }
}
