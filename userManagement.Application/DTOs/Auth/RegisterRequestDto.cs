namespace userManagement.Application.DTOs.Auth;

public sealed class RegisterRequestDto
{
    public string Username { get; init; } = string.Empty;
    public string Email    { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;

   
    public string Role     { get; init; } = "User";
}