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
        // 1️⃣ Configuración de JWT desde appsettings o variables de entorno
            var jwtKey = _configuration["Jwt:Key"] 
                         ?? Environment.GetEnvironmentVariable("SECRET_KEY");
            Console.WriteLine($"SECRET_KEY env: {Environment.GetEnvironmentVariable("SECRET_KEY")}");
            var jwtIssuer = _configuration["Jwt:Issuer"]
                            ?? Environment.GetEnvironmentVariable("ISSUER");
            var jwtAudience = _configuration["Jwt:Audience"]
                              ?? Environment.GetEnvironmentVariable("AUDIENCE");
            var jwtExpiresMinutes = int.TryParse(
                _configuration["Jwt:ExpiresMinutes"], out var minutes) ? minutes : 60;

            if (string.IsNullOrEmpty(jwtKey))
                throw new InvalidOperationException("La clave JWT (Jwt:Key o JWT_KEY) no está configurada.");

            // 2️⃣ Crear las credenciales de firma
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // 3️⃣ Crear los claims básicos del usuario
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email ?? ""),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            if (!string.IsNullOrEmpty(user.Role))
            {
                claims.Add(new Claim(ClaimTypes.Role, user.Role));
            }

            // 4️⃣ Fecha de expiración
            var expires = DateTime.UtcNow.AddMinutes(jwtExpiresMinutes);

            // 5️⃣ Crear el token
            var token = new JwtSecurityToken(
                issuer: jwtIssuer,
                audience: jwtAudience,
                claims: claims,
                expires: expires,
                signingCredentials: creds
            );

            // 6️⃣ Serializar token
            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

            return (tokenString, expires);
    }
}