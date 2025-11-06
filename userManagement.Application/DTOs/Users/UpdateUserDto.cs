namespace userManagement.Application.DTOs.Users;

public sealed class UpdateUserDto
{
    public string Email { get; init; } = string.Empty;
    public string Role { get; init; } = "User";
}