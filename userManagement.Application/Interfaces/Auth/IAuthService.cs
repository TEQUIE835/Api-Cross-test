using userManagement.Application.DTOs.Auth;

namespace userManagement.Application.Interfaces.Auth;

public interface IAuthService
{
    Task<int> RegisterAsync(RegisterRequestDto request);
    Task<LoginResponseDto> LoginAsync(LoginRequestDto request);
}