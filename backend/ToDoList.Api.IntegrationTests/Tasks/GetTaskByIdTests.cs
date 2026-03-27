using System.Net;
using System.Net.Http.Json;
using ToDoList.Api.DTOs;
using ToDoList.Api.IntegrationTests.Infrastructure;
using Xunit;

namespace ToDoList.Api.IntegrationTests.Tasks;

public class GetTaskByIdTests : TasksTestBase
{
    [Fact]
    public async Task ReturnsTask_WhenFound()
    {
        var seeded = await SeedTaskAsync("Found Task");

        var response = await Client.GetAsync($"/api/tasks/{seeded.Id}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        
        var task = await response.Content.ReadFromJsonAsync<TaskItemDto>(JsonOptions);
        Assert.NotNull(task);
        Assert.Equal(seeded.Id,    task.Id);
        Assert.Equal("Found Task", task.Title);
    }

    [Fact]
    public async Task Returns404_WhenNotFound()
    {
        var response = await Client.GetAsync("/api/tasks/99999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Returns404_ForOtherUsersTask()
    {
        var otherUser = await SeedUserAsync("Other User");
        var otherTask = await SeedTaskAsync("Other's task", otherUser.Id);

        var response = await Client.GetAsync($"/api/tasks/{otherTask.Id}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
