using System.ComponentModel.DataAnnotations;
using ToDoList.Api.Domain.Enums;

namespace ToDoList.Api.DTOs;

public class UpdateTaskStatusDto
{
    [EnumDataType(typeof(TaskItemStatus), ErrorMessage = "Invalid status value.")]
    public TaskItemStatus Status { get; set; }
}
