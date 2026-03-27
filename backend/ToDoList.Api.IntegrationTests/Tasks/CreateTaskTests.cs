using System.Net;
using System.Net.Http.Json;
using ToDoList.Api.Domain.Enums;
using ToDoList.Api.DTOs;
using ToDoList.Api.IntegrationTests.Infrastructure;
using Xunit;

namespace ToDoList.Api.IntegrationTests.Tasks;

public class CreateTaskTests : TasksTestBase
{
    [Fact]
    public async Task Returns201_WithCreatedTask()
    {
        var dto = new
        {
            title = "New Task", 
            description = "Some desc", 
            status = "Todo", 
            priority = "High"
        };

        var response = await Client.PostAsJsonAsync("/api/tasks", dto);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        
        var task = await response.Content.ReadFromJsonAsync<TaskItemDto>(JsonOptions);
        Assert.NotNull(task);
        Assert.True(task.Id > 0);
        Assert.Equal("New Task",          task.Title);
        Assert.Equal("Some desc",         task.Description);
        Assert.Equal(TaskItemStatus.Todo, task.Status);
        Assert.Equal(TaskPriority.High,   task.Priority);
    }

    [Fact]
    public async Task Returns400_WhenTitleMissing()
    {
        var dto = new
        {
            description = "No title here"
        };

        var response = await Client.PostAsJsonAsync("/api/tasks", dto);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Returns400_WhenTitleExceedsMaxLength()
    {
        var dto = new
        {
            title = new string('x', 201)
        };

        var response = await Client.PostAsJsonAsync("/api/tasks", dto);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}
