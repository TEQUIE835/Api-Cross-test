using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using userManagement.Application.DTOs.Students;
using userManagement.Application.Interfaces.Students;

namespace userManagement.Api.Controllers;

[Authorize] // Proteger todas las rutas con JWT
[ApiController]
[Route("api/[controller]")]
public class StudentsController : ControllerBase
{
    private readonly IStudentService _studentService;

    public StudentsController(IStudentService studentService)
    {
        _studentService = studentService;
    }

    /// GET /api/students
    [HttpGet]
    public async Task<ActionResult<IEnumerable<StudentDto>>> GetAll()
    {
        var list = await _studentService.GetAllAsync();
        return Ok(list);
    }

    /// GET /api/students/{id}
    [HttpGet("{id:int}")]
    public async Task<ActionResult<StudentDto>> GetById(int id)
    {
        try
        {
            var item = await _studentService.GetByIdAsync(id);
            return Ok(item);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    /// POST /api/students
    [HttpPost]
    public async Task<ActionResult<object>> Create([FromBody] CreateStudentDto input)
    {
        var newId = await _studentService.CreateAsync(input);
        return CreatedAtAction(nameof(GetById), new { id = newId }, new { id = newId });
    }

    /// PUT /api/students/{id}
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateStudentDto input)
    {
        try
        {
            await _studentService.UpdateAsync(id, input);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    /// DELETE /api/students/{id}
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            await _studentService.DeleteAsync(id);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }
}
