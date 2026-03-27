using System.Net;
using Microsoft.EntityFrameworkCore;
using ToDoList.Api.IntegrationTests.Infrastructure;
using Xunit;

namespace ToDoList.Api.IntegrationTests.Tasks;

public class DeleteTaskTests : TasksTestBase
{
    [Fact]
    public async Task Returns204_WhenDeleted()
    {
        var seeded = await SeedTaskAsync("To delete");

        var deleteResponse = await Client.DeleteAsync($"/api/tasks/{seeded.Id}");

        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);

        // Confirm the task is gone
        await using var db = OpenDb();
        Assert.False(await db.Tasks.AnyAsync(t => t.Id == seeded.Id));
    }

    [Fact]
    public async Task Returns404_WhenNotFound()
    {
        var response = await Client.DeleteAsync("/api/tasks/99999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
