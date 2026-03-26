using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ToDoList.Api.DTOs;
using ToDoList.Api.Middleware;
using ToDoList.Api.Services;

namespace ToDoList.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class TasksController(TaskService taskService) : ControllerBase
{
    private int CurrentUserId => (int)HttpContext.Items[UserIdMiddleware.UserIdKey]!;

    /// <summary>Returns all tasks for the current user, with optional filtering and sorting.</summary>
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] TaskQueryParams queryParams, CancellationToken ct)
    {
        var tasks = await taskService.GetAllAsync(queryParams, CurrentUserId, ct);
        return Ok(tasks);
    }

    /// <summary>Returns a single task by ID.</summary>
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id, CancellationToken ct)
    {
        var task = await taskService.GetByIdAsync(id, CurrentUserId, ct);
        return task is null ? TaskNotFound(id) : Ok(task);
    }

    /// <summary>Creates a new task for the current user.</summary>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateTaskDto dto, CancellationToken ct)
    {
        var created = await taskService.CreateAsync(dto, CurrentUserId, ct);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    /// <summary>Replaces an existing task.</summary>
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateTaskDto dto, CancellationToken ct)
    {
        var updated = await taskService.UpdateAsync(id, dto, CurrentUserId, ct);
        return updated is null ? TaskNotFound(id) : Ok(updated);
    }

    /// <summary>Updates only the status of a task.</summary>
    [HttpPatch("{id:int}/status")]
    public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateTaskStatusDto dto, CancellationToken ct)
    {
        var updated = await taskService.UpdateStatusAsync(id, dto.Status, CurrentUserId, ct);
        return updated is null ? TaskNotFound(id) : Ok(updated);
    }

    /// <summary>Deletes a task.</summary>
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        var deleted = await taskService.DeleteAsync(id, CurrentUserId, ct);
        return deleted ? NoContent() : TaskNotFound(id);
    }

    private ObjectResult TaskNotFound(int id) =>
        Problem(statusCode: StatusCodes.Status404NotFound, title: "Task not found.",
            detail: $"No task with ID {id} exists.");
}
