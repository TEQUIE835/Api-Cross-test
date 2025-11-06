using Microsoft.AspNetCore.Mvc;
using userManagement.Application.DTOs.Auth;
using userManagement.Application.Interfaces.Auth;

namespace userManagement.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

  
    /// POST 
    [HttpPost("register")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)] // Para username ya existente
    public async Task<IActionResult> Register([FromBody] RegisterRequestDto request)
    {
        try
        {
            var userId = await _authService.RegisterAsync(request);
            // Retorna 201 Created con la ubicación del nuevo recurso (aunque solo el ID por ahora)
            return CreatedAtAction(nameof(Register), new { id = userId });
        }
        catch (InvalidOperationException ex)
        {
            // Atrapa el error de 'Username already exists.'
            return Conflict(new { message = ex.Message });
        }
    }

  
    /// POST 
    [HttpPost("login")]
    [ProducesResponseType(typeof(LoginResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)] // Para credenciales inválidas
    public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
    {
        try
        {
            var response = await _authService.LoginAsync(request);
            return Ok(response);
        }
        catch (UnauthorizedAccessException ex)
        {
            // Atrapa el error de credentials
            return Unauthorized(new { message = ex.Message });
        }
    }
}