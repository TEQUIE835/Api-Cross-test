namespace userManagement.Application.DTOs.Auth;

public sealed class LoginResponseDto
{
    public string Token { get; init; } = string.Empty;
    public string Username { get; init; } = string.Empty;
    public string Role { get; init; } = string.Empty;
    public DateTime ExpiresAtUtc { get; init; }
}