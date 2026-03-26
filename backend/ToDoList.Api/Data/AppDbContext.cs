using Microsoft.EntityFrameworkCore;
using ToDoList.Api.Domain.Entities;

namespace ToDoList.Api.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<TaskItem> Tasks => Set<TaskItem>();
    public DbSet<User> Users => Set<User>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.GoogleId).IsUnique();
            entity.Property(e => e.GoogleId).IsRequired();
            entity.Property(e => e.Email).IsRequired();
            entity.Property(e => e.Name).IsRequired();
        });

        modelBuilder.Entity<TaskItem>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Title).IsRequired().HasMaxLength(TaskItem.TitleMaxLength);
            entity.Property(e => e.Description).HasMaxLength(TaskItem.DescriptionMaxLength);
            entity.Property(e => e.Status);
            entity.Property(e => e.Priority);
            entity.HasOne(e => e.User)
                  .WithMany()
                  .HasForeignKey(e => e.UserId)
                  .OnDelete(DeleteBehavior.Cascade);

            // Composite indexes covering the filter + sort combinations in TaskService
            entity.HasIndex(e => new { e.UserId, e.CreatedAt });   // default sort
            entity.HasIndex(e => new { e.UserId, e.DueDate });     // sort by due date
            entity.HasIndex(e => new { e.UserId, e.Status });      // filter/sort by status
            entity.HasIndex(e => new { e.UserId, e.Priority });    // filter/sort by priority
        });
    }
}
