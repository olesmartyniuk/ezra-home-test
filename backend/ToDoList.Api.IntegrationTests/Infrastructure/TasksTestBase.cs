using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.DependencyInjection;
using ToDoList.Api.Data;
using ToDoList.Api.Domain.Entities;
using ToDoList.Api.Domain.Enums;
using Xunit;

namespace ToDoList.Api.IntegrationTests.Infrastructure;

public abstract class TasksTestBase : IAsyncLifetime
{
    private readonly CustomWebApplicationFactory _factory = new();

    protected HttpClient Client { get; private set; } = null!;

    protected static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        Converters = { new JsonStringEnumConverter() }
    };

    public async Task InitializeAsync()
    {
        Client = _factory.CreateClient();
        await SeedUserAsync("Test");
    }

    public Task DisposeAsync()
    {
        _factory.Dispose();
        return Task.CompletedTask;
    }

    protected AppDbContext OpenDb()
    {
        var scope = _factory.Services.CreateScope();
        return scope.ServiceProvider.GetRequiredService<AppDbContext>();
    }

    protected async Task<TaskItem> SeedTaskAsync(
        string         title       = "Test Task",
        int?           userId      = null,
        TaskItemStatus status      = TaskItemStatus.Todo,
        TaskPriority   priority    = TaskPriority.Medium,
        string?        description = null,
        DateTime?      dueDate     = null)
    {
        await using var db = OpenDb();
        var task = new TaskItem
        {
            UserId      = userId ?? FakeAuthHandler.TestUserId,
            Title       = title,
            Description = description,
            Status      = status,
            Priority    = priority,
            DueDate     = dueDate,
            CreatedAt   = DateTime.UtcNow,
            UpdatedAt   = DateTime.UtcNow
        };
        db.Tasks.Add(task);
        await db.SaveChangesAsync();
        return task;
    }

    protected async Task<User> SeedUserAsync(string uniqueName)
    {
        await using var db = OpenDb();
        var user = new User($"{uniqueName}-google-id", $"{uniqueName}@example.com", $"{uniqueName} User");
        db.Users.Add(user);
        await db.SaveChangesAsync();
        return user;
    }
}
