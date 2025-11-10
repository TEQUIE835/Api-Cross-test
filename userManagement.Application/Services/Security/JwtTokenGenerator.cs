using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using userManagement.Application.Interfaces.Security;
using userManagement.Domain.Entities;

namespace userManagement.Application.Services.Security;

public class JwtTokenGenerator : IJwtTokenGenerator
{
    private readonly IConfiguration _configuration;

    public JwtTokenGenerator(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public (string token, DateTime expiresAtUtc) GenerateToken(User user)
    {
        // 1) Leer configuración
        var jwtKey =
            _configuration["Jwt:Key"] ??
            _configuration["SecretKey"] ??
            Environment.GetEnvironmentVariable("SECRET_KEY");
        var jwtIssuer =
            _configuration["Jwt:Issuer"] ??
            _configuration["Issuer"] ??
            Environment.GetEnvironmentVariable("ISSUER");
        var jwtAudience =
            _configuration["Jwt:Audience"] ??
            _configuration["Audience"] ??
            Environment.GetEnvironmentVariable("AUDIENCE");

        if (string.IsNullOrWhiteSpace(jwtKey))
            throw new InvalidOperationException("JWT Key is not configured.");

        // 2) Credenciales de firma
        var key  = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        // 3) Claims del usuario (incluye NameIdentifier y Role)
        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()), // <- necesario para CurrentUserService.UserId
            new Claim(JwtRegisteredClaimNames.Email, user.Email ?? string.Empty),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        if (!string.IsNullOrWhiteSpace(user.Role))
        {
            claims.Add(new Claim(ClaimTypes.Role, user.Role));        // <- necesario para [Authorize(Roles="Admin")]
        }

        // 4) Expiración (60 min por defecto)
        var minutes = 60;
        if (int.TryParse(_configuration["Jwt:ExpiresMinutes"], out var cfgMinutes) && cfgMinutes > 0)
            minutes = cfgMinutes;

        var expires = DateTime.UtcNow.AddMinutes(minutes);

        // 5) Crear el token
        var token = new JwtSecurityToken(
            issuer: jwtIssuer,
            audience: jwtAudience,
            claims: claims,
            expires: expires,
            signingCredentials: creds
        );

        // 6) Serializar token
        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

        return (tokenString, expires);
    }
}
