using System.ComponentModel.DataAnnotations;
using ToDoList.Api.Domain.Enums;

namespace ToDoList.Api.DTOs;

public class UpdateTaskDto
{
    [Required(ErrorMessage = "Title is required.")]
    [StringLength(200, ErrorMessage = "Title must not exceed 200 characters.")]
    public string Title { get; set; } = string.Empty;

    [StringLength(2000, ErrorMessage = "Description must not exceed 2000 characters.")]
    public string? Description { get; set; }

    [EnumDataType(typeof(TaskItemStatus), ErrorMessage = "Invalid status value.")]
    public TaskItemStatus Status { get; set; }

    [EnumDataType(typeof(TaskPriority), ErrorMessage = "Invalid priority value.")]
    public TaskPriority Priority { get; set; }

    public DateTime? DueDate { get; set; }
}
