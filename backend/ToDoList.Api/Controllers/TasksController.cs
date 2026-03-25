using Microsoft.AspNetCore.Mvc;
using ToDoList.Api.DTOs;
using ToDoList.Api.Services.Interfaces;

namespace ToDoList.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TasksController : ControllerBase
{
    private readonly ITaskService _taskService;

    public TasksController(ITaskService taskService) => _taskService = taskService;

    /// <summary>Returns all tasks, with optional filtering and sorting.</summary>
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] TaskQueryParams queryParams)
    {
        var tasks = await _taskService.GetAllAsync(queryParams);
        return Ok(tasks);
    }

    /// <summary>Returns a single task by ID.</summary>
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var task = await _taskService.GetByIdAsync(id);
        return task is null
            ? NotFound(new { status = 404, title = "Task not found.", detail = $"No task with ID {id} exists." })
            : Ok(task);
    }

    /// <summary>Creates a new task.</summary>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateTaskDto dto)
    {
        throw new NotImplementedException("This method is not implemented yet. Please implement the CreateAsync method in TaskService and then uncomment this code.");
        var created = await _taskService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    /// <summary>Replaces an existing task.</summary>
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateTaskDto dto)
    {
        var updated = await _taskService.UpdateAsync(id, dto);
        return updated is null
            ? NotFound(new { status = 404, title = "Task not found.", detail = $"No task with ID {id} exists." })
            : Ok(updated);
    }

    /// <summary>Updates only the status of a task.</summary>
    [HttpPatch("{id:int}/status")]
    public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateTaskStatusDto dto)
    {
        var updated = await _taskService.UpdateStatusAsync(id, dto.Status);
        return updated is null
            ? NotFound(new { status = 404, title = "Task not found.", detail = $"No task with ID {id} exists." })
            : Ok(updated);
    }

    /// <summary>Deletes a task.</summary>
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _taskService.DeleteAsync(id);
        return deleted
            ? NoContent()
            : NotFound(new { status = 404, title = "Task not found.", detail = $"No task with ID {id} exists." });
    }
}
