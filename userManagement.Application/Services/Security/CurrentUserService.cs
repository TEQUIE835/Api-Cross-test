using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using userManagement.Application.Interfaces.Security;

namespace userManagement.Application.Services.Security;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    
    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }
    
    private ClaimsPrincipal? User => _httpContextAccessor.HttpContext?.User;

    public int? UserId
    {
        get
        {
            // Busca el ClaimTypes.NameIdentifier (el ID del usuario)
            var idClaim = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            
            // Intenta parsear el valor a int
            if (idClaim is not null && int.TryParse(idClaim, out var userId))
            {
                return userId;
            }
            return null;
        }
    }
    public string? Username => User?.FindFirst(ClaimTypes.Name)?.Value;

    public string? Role => User?.FindFirst(ClaimTypes.Role)?.Value;
}