using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using userManagement.Application.DTOs.Users;
using userManagement.Application.Interfaces.Users;

namespace userManagement.Api.Controllers;


[Authorize] 
[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }
    
    /// GET /api/users - Lista todos los usuarios (protegido, rol Admin).
    [HttpGet]
    [Authorize(Roles = "Admin")] // Específicamente requiere el rol Admin
    public async Task<ActionResult<IEnumerable<UserDto>>> GetAll()
    {
        var users = await _userService.GetAllAsync();
        return Ok(users);
    }

    
    /// GET /api/users/{id} - Obtiene un usuario por ID (protegido).
    [HttpGet("{id:int}")]
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
    

   
    /// PUT /api/users/{id} - Actualiza un usuario existente (protegido).
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

  
    /// DELETE /api/users/{id} - Elimina un usuario (protegido, rol Admin).
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