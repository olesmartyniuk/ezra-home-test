using Microsoft.EntityFrameworkCore;
using ToDoList.Api.Domain.Entities;

namespace ToDoList.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<TaskItem> Tasks => Set<TaskItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TaskItem>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Title).IsRequired().HasMaxLength(TaskItem.TitleMaxLength);
            entity.Property(e => e.Description).HasMaxLength(TaskItem.DescriptionMaxLength);
            // Store enums as strings for readability in SQLite
            entity.Property(e => e.Status).HasConversion<string>();
            entity.Property(e => e.Priority).HasConversion<string>();
        });
    }
}
