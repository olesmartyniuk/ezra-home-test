using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ToDoList.Api.Data;

namespace ToDoList.Api.IntegrationTests.Infrastructure;

/// <summary>
/// Hosts the API in-process using an isolated SQLite in-memory database
/// and a fake authentication handler so tests do not require real JWTs.
/// </summary>
public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    private readonly SqliteConnection _connection = new("Data Source=:memory:");

    public CustomWebApplicationFactory()
    {
        _connection.Open();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        // Provide all required configuration so Program.cs does not throw.
        builder.ConfigureAppConfiguration(config =>
        {
            config.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Jwt:Secret"]              = new('x', 64),
                ["Jwt:Issuer"]              = "test",
                ["Jwt:Audience"]            = "test",
                ["Cors:AllowedOrigin"]      = "http://localhost:5173"
            });
        });

        builder.ConfigureTestServices(services =>
        {
            // Replace the real DbContext (SQLite file) with in-memory SQLite.
            var descriptor = services.Single(
                d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));
            services.Remove(descriptor);

            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlite(_connection));

            // Replace JWT bearer with a handler that always authenticates as TestUserId.
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = FakeAuthHandler.SchemeName;
                options.DefaultChallengeScheme    = FakeAuthHandler.SchemeName;
            })
            .AddScheme<AuthenticationSchemeOptions, FakeAuthHandler>(
                FakeAuthHandler.SchemeName, _ => { });
        });
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
            _connection.Dispose();

        base.Dispose(disposing);
    }
}
