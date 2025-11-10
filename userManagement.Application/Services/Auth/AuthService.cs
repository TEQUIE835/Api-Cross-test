using userManagement.Application.DTOs.Auth;
using userManagement.Application.Interfaces.Auth;
using userManagement.Application.Interfaces.Security;
using userManagement.Domain.Entities;
using userManagement.Domain.Interfaces;

namespace userManagement.Application.Services.Auth;

public class AuthService : IAuthService
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
        // Normalización básica
        var username = request.Username.Trim();
        var email    = request.Email.Trim().ToLowerInvariant();

        // Verificar username único (y opcional: email único si quieres)
        var existingByUsername = await _users.GetUserByUsername(username);
        if (existingByUsername is not null)
            throw new InvalidOperationException("Username already exists.");

       
        var existingByEmail = await _users.GetUserByEmail(email);
        if (existingByEmail is not null)
         throw new InvalidOperationException("Email already exists.");

        var allowed = new[] { "User", "Admin" };
        var role = allowed.Contains(request.Role) ? request.Role : "User";

        var user = new User
        {
            Username     = username,
            Email        = email,
            PasswordHash = _hasher.Hash(request.Password),
            Role         = role
        };

        await _users.RegisterUser(user);
        return user.Id;
    }

    public async Task<LoginResponseDto> LoginAsync(LoginRequestDto request)
    {
        // Login acepta username O email (UsernameOrEmail)
        var identifier = request.UsernameOrEmail.Trim().ToLowerInvariant();

        User? user = identifier.Contains('@')
            ? await _users.GetUserByEmail(identifier)
            : await _users.GetUserByUsername(identifier);

        if (user is null)
            throw new UnauthorizedAccessException("Invalid credentials.");

        if (!_hasher.Verify(request.Password, user.PasswordHash))
            throw new UnauthorizedAccessException("Invalid credentials.");

        var (token, expiresAtUtc) = _jwt.GenerateToken(user);

        return new LoginResponseDto
        {
            Token        = token,
            ExpiresAtUtc = expiresAtUtc,
            Username     = user.Username,
            Role         = user.Role
        };
    }
}
