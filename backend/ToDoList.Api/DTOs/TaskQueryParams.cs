using ToDoList.Api.Domain.Enums;

namespace ToDoList.Api.DTOs;

public class TaskQueryParams
{
    public TaskItemStatus? Status { get; set; }
    public TaskPriority? Priority { get; set; }
    public string? SortBy { get; set; } = "createdAt";
    public string? SortOrder { get; set; } = "desc";
    public string? Search { get; set; }
}
