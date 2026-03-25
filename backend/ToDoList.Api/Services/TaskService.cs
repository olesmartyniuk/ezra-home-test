using Microsoft.EntityFrameworkCore;
using ToDoList.Api.Data;
using ToDoList.Api.Domain.Entities;
using ToDoList.Api.Domain.Enums;
using ToDoList.Api.DTOs;
namespace ToDoList.Api.Services;

public class TaskService
{
    private readonly AppDbContext _db;

    public TaskService(AppDbContext db) => _db = db;

    public async Task<IEnumerable<TaskItemDto>> GetAllAsync(TaskQueryParams queryParams, CancellationToken ct = default)
    {
        var query = _db.Tasks.AsQueryable();

        if (queryParams.Status.HasValue)
            query = query.Where(t => t.Status == queryParams.Status.Value);

        if (queryParams.Priority.HasValue)
            query = query.Where(t => t.Priority == queryParams.Priority.Value);

        if (!string.IsNullOrWhiteSpace(queryParams.Search))
        {
            var search = queryParams.Search.ToLower();
            query = query.Where(t =>
                t.Title.ToLower().Contains(search) ||
                (t.Description != null && t.Description.ToLower().Contains(search)));
        }

        query = (queryParams.SortBy?.ToLower(), queryParams.SortOrder?.ToLower()) switch
        {
            ("duedate", "asc")     => query.OrderBy(t => t.DueDate),
            ("duedate", _)         => query.OrderByDescending(t => t.DueDate),
            ("priority", "asc")    => query.OrderBy(t => t.Priority),
            ("priority", _)        => query.OrderByDescending(t => t.Priority),
            ("status", "asc")      => query.OrderBy(t => t.Status),
            ("status", _)          => query.OrderByDescending(t => t.Status),
            ("createdat", "asc")   => query.OrderBy(t => t.CreatedAt),
            _                      => query.OrderByDescending(t => t.CreatedAt)
        };

        var tasks = await query.ToListAsync(ct);
        return tasks.Select(MapToDto);
    }

    public async Task<TaskItemDto?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var task = await _db.Tasks.FindAsync([id], ct);
        return task is null ? null : MapToDto(task);
    }

    public async Task<TaskItemDto> CreateAsync(CreateTaskDto dto, CancellationToken ct = default)
    {
        var now = DateTime.UtcNow;
        var task = new TaskItem
        {
            Title = dto.Title,
            Description = dto.Description,
            Status = dto.Status,
            Priority = dto.Priority,
            DueDate = dto.DueDate,
            CreatedAt = now,
            UpdatedAt = now
        };

        _db.Tasks.Add(task);
        await _db.SaveChangesAsync(ct);
        return MapToDto(task);
    }

    public async Task<TaskItemDto?> UpdateAsync(int id, UpdateTaskDto dto, CancellationToken ct = default)
    {
        var task = await _db.Tasks.FindAsync([id], ct);
        if (task is null) return null;

        task.Title = dto.Title;
        task.Description = dto.Description;
        task.Status = dto.Status;
        task.Priority = dto.Priority;
        task.DueDate = dto.DueDate;
        task.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync(ct);
        return MapToDto(task);
    }

    public async Task<TaskItemDto?> UpdateStatusAsync(int id, TaskItemStatus status, CancellationToken ct = default)
    {
        var task = await _db.Tasks.FindAsync([id], ct);
        if (task is null) return null;

        task.Status = status;
        task.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync(ct);
        return MapToDto(task);
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
    {
        var task = await _db.Tasks.FindAsync([id], ct);
        if (task is null) return false;

        _db.Tasks.Remove(task);
        await _db.SaveChangesAsync(ct);
        return true;
    }

    private static TaskItemDto MapToDto(TaskItem task) => new()
    {
        Id = task.Id,
        Title = task.Title,
        Description = task.Description,
        Status = task.Status,
        Priority = task.Priority,
        DueDate = task.DueDate,
        CreatedAt = task.CreatedAt,
        UpdatedAt = task.UpdatedAt
    };
}
