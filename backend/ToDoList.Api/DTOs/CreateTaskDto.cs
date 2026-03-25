using System.ComponentModel.DataAnnotations;
using ToDoList.Api.Domain.Entities;
using ToDoList.Api.Domain.Enums;

namespace ToDoList.Api.DTOs;

public class CreateTaskDto
{
    [Required(ErrorMessage = "Title is required.")]
    [StringLength(TaskItem.TitleMaxLength, ErrorMessage = "Title must not exceed 200 characters.")]
    public string Title { get; set; } = string.Empty;

    [StringLength(TaskItem.DescriptionMaxLength, ErrorMessage = "Description must not exceed 2000 characters.")]
    public string? Description { get; set; }

    [EnumDataType(typeof(TaskItemStatus), ErrorMessage = "Invalid status value.")]
    public TaskItemStatus Status { get; set; } = TaskItemStatus.Todo;

    [EnumDataType(typeof(TaskPriority), ErrorMessage = "Invalid priority value.")]
    public TaskPriority Priority { get; set; } = TaskPriority.Medium;

    public DateTime? DueDate { get; set; }
}
