using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using userManagement.Application.DTOs.Auth;
using userManagement.Application.Interfaces.Auth;

namespace userManagement.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class AuthController : ControllerBase
{
    private readonly IAuthService _auth;

    public AuthController(IAuthService auth)
    {
        _auth = auth;
    }

    /// <summary>Registro de usuario.</summary>
    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<IActionResult> Register([FromBody] RegisterRequestDto request)
    {
        // IAuthService.RegisterAsync devuelve el nuevo Id (int)
        var newId = await _auth.RegisterAsync(request);
        // 201 Created con Location -> /api/users/{id}
        return CreatedAtRoute(
            routeName: "GetUserById",
            routeValues: new { id = newId },
            value: new { id = newId }
        );
    }

    /// <summary>Login: devuelve JWT y metadatos.</summary>
    [HttpPost("login")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
    {
        try
        {
            var response = await _auth.LoginAsync(request); // LoginResponseDto
            return Ok(response);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
    }
}