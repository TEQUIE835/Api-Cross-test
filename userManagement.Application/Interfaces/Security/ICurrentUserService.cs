namespace userManagement.Application.Interfaces.Security;

public interface ICurrentUserService
{
    int? UserId { get; }
    string? Username { get; }
    string? Role { get; }
}