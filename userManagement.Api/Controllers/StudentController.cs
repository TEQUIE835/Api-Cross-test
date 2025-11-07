using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using userManagement.Application.DTOs.Students;
using userManagement.Application.Interfaces.Students;

namespace userManagement.Api.Controllers;


[ApiController]
[Route("api/[controller]")]
public class StudentsController : ControllerBase
{
    private readonly IStudentService _studentService;

    public StudentsController(IStudentService studentService)
    {
        _studentService = studentService;
    }

    
    /// GET 
    [HttpGet]
    public async Task<ActionResult<IEnumerable<StudentDto>>> GetAll()
    {
        try{
            var students = await _studentService.GetAllAsync();
            return Ok(students);
        }
        catch ( Exception e){
            return BadRequest(e.Message);
        }
    }

    
    /// GET 
    [HttpGet("{id:int}")]
    public async Task<ActionResult<StudentDto>> GetById(int id)
    {
        try
        {
            var student = await _studentService.GetByIdAsync(id);
            return Ok(student);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    
    /// POST /api/students - Crea un nuevo estudiante (protegido).
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateStudentDto input)
    {
        var id = await _studentService.CreateAsync(input);
        return CreatedAtAction(nameof(GetById), new { id }, null);
    }

    
    /// PUT /api/students/{id} - Actualiza un estudiante existente (protegido).
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

  
    /// DELETE 
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
            // Si el servicio no lanza excepción
            // Si quieres confirmar la eliminación
            return NotFound();
        }
    }
}
