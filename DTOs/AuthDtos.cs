namespace OurBigDay.Api.DTOs;

public record LoginRequest(string Email, string Password);

public record RegisterRequest(string Email, string Password, string DisplayName, string? Role = null);

public record AuthResponse(string Token, string Email, string Role, string DisplayName);
