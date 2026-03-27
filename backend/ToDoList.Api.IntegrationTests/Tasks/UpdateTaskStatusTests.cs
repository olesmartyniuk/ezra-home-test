using System.Net;
using System.Net.Http.Json;
using ToDoList.Api.Domain.Enums;
using ToDoList.Api.DTOs;
using ToDoList.Api.IntegrationTests.Infrastructure;
using Xunit;

namespace ToDoList.Api.IntegrationTests.Tasks;

public class UpdateTaskStatusTests : TasksTestBase
{
    [Fact]
    public async Task Returns200_WithNewStatus()
    {
        var seeded = await SeedTaskAsync("Task");
        var dto = new { status = "Done" };

        var response = await Client.PatchAsJsonAsync($"/api/tasks/{seeded.Id}/status", dto);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var task = await response.Content.ReadFromJsonAsync<TaskItemDto>(JsonOptions);
        Assert.NotNull(task);
        Assert.Equal(TaskItemStatus.Done, task.Status);
    }

    [Fact]
    public async Task Returns404_WhenNotFound()
    {
        var dto = new { status = "Done" };

        var response = await Client.PatchAsJsonAsync("/api/tasks/99999/status", dto);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
