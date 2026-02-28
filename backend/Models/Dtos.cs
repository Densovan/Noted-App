namespace NotesApp.Api.Models;

public record UserRegisterRequest(string Username, string Password);
public record UserLoginRequest(string Username, string Password);
public record AuthResponse(string Token, string Username);

public record NoteCreateRequest(string Title, string? Content);
public record NoteUpdateRequest(string Title, string? Content);
public record NoteResponse(int Id, string Title, string? Content, DateTime CreatedAt, DateTime UpdatedAt);
