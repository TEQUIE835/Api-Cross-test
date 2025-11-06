using userManagement.Application.DTOs.Users;
using userManagement.Application.Interfaces.Users;
using userManagement.Application.Interfaces.Security;
using userManagement.Domain.Entities;
using userManagement.Domain.Interfaces;

namespace userManagement.Application.Services.Users;

public sealed class UserService : IUserService
{
    private readonly IUserRepository _users;
    private readonly ICurrentUserService _current;

    public UserService(IUserRepository users, ICurrentUserService current)
    {
        _users = users;
        _current = current;
    }

    private void EnsureAuthenticated()
    {
        if (_current.UserId is null)
            throw new UnauthorizedAccessException("Authentication required.");
    }

    private bool IsAdmin() =>
        string.Equals(_current.Role, "Admin", StringComparison.OrdinalIgnoreCase);

    public async Task<IEnumerable<UserDto>> GetAllAsync()
    {
        EnsureAuthenticated();
        if (!IsAdmin())
            throw new UnauthorizedAccessException("Admin only.");

        var entities = await _users.GetAllUsers();
        return entities.Select(MapToDto);
    }

    public async Task<UserDto> GetByIdAsync(int id)
    {
        EnsureAuthenticated();

        var entity = await _users.GetUserById(id);
        if (entity is null) throw new KeyNotFoundException("User not found.");

        // Admin o el propio usuario
        if (!IsAdmin() && _current.UserId != entity.Id)
            throw new UnauthorizedAccessException("Forbidden.");

        return MapToDto(entity);
    }

    public async Task<int> CreateAsync(CreateUserDto input)
    {
        EnsureAuthenticated();
        if (!IsAdmin())
            throw new UnauthorizedAccessException("Admin only.");

        // Nota: para crear usuarios con hashing, usa AuthService.RegisterAsync.
        var user = new User
        {
            Username = input.Username.Trim(),
            Email = input.Email.Trim().ToLowerInvariant(),
            PasswordHash = input.Password, // se reemplaza por hash si usas AuthService
            Role = string.IsNullOrWhiteSpace(input.Role) ? "User" : input.Role.Trim()
        };

        await _users.RegisterUser(user);
        return user.Id;
    }

    public async Task UpdateAsync(int id, UpdateUserDto input)
    {
        EnsureAuthenticated();

        var user = await _users.GetUserById(id);
        if (user is null) throw new KeyNotFoundException("User not found.");

        // Admin o el propio usuario
        if (!IsAdmin() && _current.UserId != id)
            throw new UnauthorizedAccessException("Forbidden.");

        user.Email = input.Email.Trim().ToLowerInvariant();

        // Solo Admin puede cambiar el Role
        if (IsAdmin() && !string.IsNullOrWhiteSpace(input.Role))
            user.Role = input.Role.Trim();

        await _users.UpdateUser(user);
    }

    public async Task DeleteAsync(int id)
    {
        EnsureAuthenticated();
        if (!IsAdmin())
            throw new UnauthorizedAccessException("Admin only.");

        await _users.DeleteUser(id);
    }

    private static UserDto MapToDto(User u) => new()
    {
        Id = u.Id,
        Username = u.Username,
        Email = u.Email,
        Role = u.Role
    };
}
