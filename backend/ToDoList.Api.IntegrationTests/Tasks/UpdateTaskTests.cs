using System.Net;
using System.Net.Http.Json;
using ToDoList.Api.Domain.Enums;
using ToDoList.Api.DTOs;
using ToDoList.Api.IntegrationTests.Infrastructure;
using Xunit;

namespace ToDoList.Api.IntegrationTests.Tasks;

public class UpdateTaskTests : TasksTestBase
{
    [Fact]
    public async Task Returns200_WithUpdatedTask()
    {
        var seeded = await SeedTaskAsync("Original");
        var dto = new
        {
            title = "Updated",
            status = "InProgress",
            priority = "High"
        };

        var response = await Client.PutAsJsonAsync($"/api/tasks/{seeded.Id}", dto);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var task = await response.Content.ReadFromJsonAsync<TaskItemDto>(JsonOptions);
        Assert.NotNull(task);
        Assert.Equal("Updated", task.Title);
        Assert.Equal(TaskItemStatus.InProgress, task.Status);
        Assert.Equal(TaskPriority.High, task.Priority);
    }

    [Fact]
    public async Task Returns404_WhenNotFound()
    {
        var dto = new
        {
            title = "Updated", 
            status = "Todo", 
            priority = "Medium"
        };

        var response = await Client.PutAsJsonAsync("/api/tasks/99999", dto);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
