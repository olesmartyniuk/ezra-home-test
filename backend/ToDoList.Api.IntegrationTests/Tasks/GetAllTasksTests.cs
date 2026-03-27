using System.Net.Http.Json;
using ToDoList.Api.Domain.Enums;
using ToDoList.Api.DTOs;
using ToDoList.Api.IntegrationTests.Infrastructure;
using Xunit;

namespace ToDoList.Api.IntegrationTests.Tasks;

public class GetAllTasksTests : TasksTestBase
{
    [Fact]
    public async Task ReturnsEmptyList_WhenNoTasks()
    {
        var tasks = await Client.GetFromJsonAsync<TaskItemDto[]>("/api/tasks", JsonOptions);

        Assert.NotNull(tasks);
        Assert.Empty(tasks);
    }

    [Fact]
    public async Task ReturnsTasks_ForCurrentUser()
    {
        await SeedTaskAsync("Task A");
        await SeedTaskAsync("Task B");

        var tasks = await Client.GetFromJsonAsync<TaskItemDto[]>("/api/tasks", JsonOptions);

        Assert.NotNull(tasks);
        Assert.Equal(2, tasks.Length);
        Assert.Equal("Task B", tasks[0].Title);
        Assert.Equal("Task A", tasks[1].Title);
    }

    [Fact]
    public async Task ExcludesTasks_ForOtherUser()
    {
        var otherUser = await SeedUserAsync("Other User");
        await SeedTaskAsync("My Task");
        await SeedTaskAsync("Their Task", otherUser.Id);

        var tasks = await Client.GetFromJsonAsync<TaskItemDto[]>("/api/tasks", JsonOptions);

        Assert.NotNull(tasks);
        Assert.Single(tasks);
        Assert.Equal("My Task", tasks[0].Title);
    }

    [Fact]
    public async Task FiltersByStatus()
    {
        await SeedTaskAsync("Todo Task", status: TaskItemStatus.Todo);
        await SeedTaskAsync("Done Task", status: TaskItemStatus.Done);

        var tasks = await Client.GetFromJsonAsync<TaskItemDto[]>(
            "/api/tasks?status=Todo", JsonOptions);

        Assert.NotNull(tasks);
        Assert.Single(tasks);
        Assert.Equal(TaskItemStatus.Todo, tasks[0].Status);
        Assert.Equal("Todo Task", tasks[0].Title);
    }

    [Fact]
    public async Task FiltersByPriority()
    {
        await SeedTaskAsync("High Task", priority: TaskPriority.High);
        await SeedTaskAsync("Low Task",  priority: TaskPriority.Low);

        var tasks = await Client.GetFromJsonAsync<TaskItemDto[]>(
            "/api/tasks?priority=High", JsonOptions);

        Assert.NotNull(tasks);
        Assert.Single(tasks);
        Assert.Equal(TaskPriority.High, tasks[0].Priority);
        Assert.Equal("High Task", tasks[0].Title);
    }

    [Fact]
    public async Task SearchMatchesTitle()
    {
        await SeedTaskAsync("Buy groceries");
        await SeedTaskAsync("Fix bug");

        var tasks = await Client.GetFromJsonAsync<TaskItemDto[]>(
            "/api/tasks?search=groceries", JsonOptions);

        Assert.NotNull(tasks);
        Assert.Single(tasks);
        Assert.Equal("Buy groceries", tasks[0].Title);
    }

    [Fact]
    public async Task SearchMatchesDescription()
    {
        await SeedTaskAsync("Task A", description: "needs peer review");
        await SeedTaskAsync("Task B", description: "unrelated work");

        var tasks = await Client.GetFromJsonAsync<TaskItemDto[]>(
            "/api/tasks?search=peer+review", JsonOptions);

        Assert.NotNull(tasks);
        Assert.Single(tasks);
        Assert.Equal("Task A", tasks[0].Title);
    }

    [Fact]
    public async Task SortsByDueDateAsc()
    {
        var now = DateTime.UtcNow;
        await SeedTaskAsync("Later",   dueDate: now.AddDays(2));
        await SeedTaskAsync("Earlier", dueDate: now.AddDays(1));

        var tasks = await Client.GetFromJsonAsync<TaskItemDto[]>(
            "/api/tasks?sortBy=dueDate&sortOrder=asc", JsonOptions);

        Assert.NotNull(tasks);
        Assert.Equal(2, tasks.Length);
        Assert.Equal("Earlier", tasks[0].Title);
        Assert.Equal("Later",   tasks[1].Title);
    }
}
