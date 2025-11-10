using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using userManagement.Application.DTOs.Users;
using userManagement.Application.Interfaces.Users;

namespace userManagement.Api.Controllers;

[Authorize] // protegido por JWT
[ApiController]
[Route("api/[controller]")]
public sealed class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    /// <summary>Crea un usuario (solo Admin).</summary>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create([FromBody] CreateUserDto input)
    {
        var newId = await _userService.CreateAsync(input);
        return CreatedAtRoute(
            routeName: "GetUserById",
            routeValues: new { id = newId },
            value: new { id = newId }
        );
    }

    /// <summary>Lista todos los usuarios (solo Admin).</summary>
    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<IEnumerable<UserDto>>> GetAll()
    {
        var users = await _userService.GetAllAsync();
        return Ok(users);
    }

    /// <summary>Obtiene un usuario por Id. Admin o dueño del recurso (se valida en Application).</summary>
    [HttpGet("{id:int}", Name = "GetUserById")]
    public async Task<ActionResult<UserDto>> GetById(int id)
    {
        try
        {
            var user = await _userService.GetByIdAsync(id);
            return Ok(user);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
    }

    /// <summary>Actualiza un usuario. Admin o dueño del recurso.</summary>
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateUserDto input)
    {
        try
        {
            await _userService.UpdateAsync(id, input);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
    }

    /// <summary>Elimina un usuario (solo Admin).</summary>
    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            await _userService.DeleteAsync(id);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }
}
