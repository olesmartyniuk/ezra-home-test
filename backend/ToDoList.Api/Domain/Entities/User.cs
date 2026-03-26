namespace ToDoList.Api.Domain.Entities;

public class User(string googleId, string email, string name, string? picture = null)
{
    public int Id { get; set; }
    public string GoogleId { get; set; } = googleId;
    public string Email { get; set; } = email;
    public string Name { get; set; } = name;
    public string? Picture { get; set; } = picture;
}
