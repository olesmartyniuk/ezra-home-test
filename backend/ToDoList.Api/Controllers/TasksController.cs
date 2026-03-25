using Microsoft.AspNetCore.Mvc;
using ToDoList.Api.DTOs;
using ToDoList.Api.Services;

namespace ToDoList.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TasksController : ControllerBase
{
    private readonly TaskService _taskService;

    public TasksController(TaskService taskService) => _taskService = taskService;

    /// <summary>Returns all tasks, with optional filtering and sorting.</summary>
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] TaskQueryParams queryParams, CancellationToken ct)
    {
        var tasks = await _taskService.GetAllAsync(queryParams, ct);
        return Ok(tasks);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id, CancellationToken ct)
    {
        var task = await _taskService.GetByIdAsync(id, ct);
        return task is null ? TaskNotFound(id) : Ok(task);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateTaskDto dto, CancellationToken ct)
    {
        var created = await _taskService.CreateAsync(dto, ct);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateTaskDto dto, CancellationToken ct)
    {
        var updated = await _taskService.UpdateAsync(id, dto, ct);
        return updated is null ? TaskNotFound(id) : Ok(updated);
    }

    [HttpPatch("{id:int}/status")]
    public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateTaskStatusDto dto, CancellationToken ct)
    {
        var updated = await _taskService.UpdateStatusAsync(id, dto.Status, ct);
        return updated is null ? TaskNotFound(id) : Ok(updated);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        var deleted = await _taskService.DeleteAsync(id, ct);
        return deleted ? NoContent() : TaskNotFound(id);
    }

    private NotFoundObjectResult TaskNotFound(int id) =>
        NotFound(new { status = 404, title = "Task not found.", detail = $"No task with ID {id} exists." });
}
