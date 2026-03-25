using ToDoList.Api.Domain.Enums;
using ToDoList.Api.DTOs;

namespace ToDoList.Api.Services.Interfaces;

public interface ITaskService
{
    Task<IEnumerable<TaskItemDto>> GetAllAsync(TaskQueryParams queryParams);
    Task<TaskItemDto?> GetByIdAsync(int id);
    Task<TaskItemDto> CreateAsync(CreateTaskDto dto);
    Task<TaskItemDto?> UpdateAsync(int id, UpdateTaskDto dto);
    Task<TaskItemDto?> UpdateStatusAsync(int id, TaskItemStatus status);
    Task<bool> DeleteAsync(int id);
}
