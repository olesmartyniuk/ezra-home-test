using ToDoList.Api.Domain.Enums;

namespace ToDoList.Api.DTOs;

public class UpdateTaskStatusDto
{
    public TaskItemStatus Status { get; set; }
}
