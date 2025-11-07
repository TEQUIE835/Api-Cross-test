using userManagement.Application.DTOs.Auth;
using userManagement.Application.Interfaces.Auth;
using userManagement.Application.Interfaces.Security;
using userManagement.Domain.Entities;
using userManagement.Domain.Interfaces;

namespace userManagement.Application.Services.Auth;

public sealed class AuthService : IAuthService
{
    private readonly IUserRepository _users;
    private readonly IPasswordHasher _hasher;
    private readonly IJwtTokenGenerator _jwt;

    public AuthService(IUserRepository users, IPasswordHasher hasher, IJwtTokenGenerator jwt)
    {
        _users = users;
        _hasher = hasher;
        _jwt = jwt;
    }

    public async Task<int> RegisterAsync(RegisterRequestDto request)
    {
        
        var existing = await _users.GetUserById(request.Username);
        if (existing is not null)
            throw new InvalidOperationException("Username already exists.");

        var user = new User
        {
            Username = request.Username.Trim(),
            Email = request.Email.Trim().ToLowerInvariant(),
            PasswordHash = _hasher.Hash(request.Password),
            Role = "User"
        };

        await _users.RegisterUser(user);
        return user.Id;
    }

    public async Task<LoginResponseDto> LoginAsync(LoginRequestDto request)
    {
        // Login por Username (si agregas GetByEmail, puedes aceptar email también)
        var user = await _users.GetUserById(request.UsernameOrEmail);
        if (user is null)
            throw new UnauthorizedAccessException("Invalid credentials.");

        if (!_hasher.Verify(request.Password, user.PasswordHash))
            throw new UnauthorizedAccessException("Invalid credentials.");

        var (token, expiresAtUtc) = _jwt.GenerateToken(user);

        return new LoginResponseDto
        {
            Token = token,
            ExpiresAtUtc = expiresAtUtc,
            Username = user.Username,
            Role = user.Role
        };
    }
}
