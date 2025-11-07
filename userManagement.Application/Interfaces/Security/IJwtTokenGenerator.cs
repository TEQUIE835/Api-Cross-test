using userManagement.Domain.Entities;

namespace userManagement.Application.Interfaces.Security;

public interface IJwtTokenGenerator
{
    /// <summary>
    /// Genera el token JWT basado en el usuario autenticado.
    /// Devuelve (token, fechaExpiraciónUTC)
    /// </summary>
    (string token, DateTime expiresAtUtc) GenerateToken(User user);
}