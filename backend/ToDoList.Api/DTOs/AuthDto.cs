namespace ToDoList.Api.DTOs;

public record GoogleAuthRequest(string IdToken);
public record AuthUserDto(int Id, string Email, string Name, string? Picture);
public record AuthResponse(string Token, AuthUserDto User);
