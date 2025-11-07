using userManagement.Application.DTOs.Students;
using userManagement.Application.Interfaces.Students;
using userManagement.Application.Interfaces.Security;
using userManagement.Domain.Entities;
using userManagement.Domain.Interfaces;

namespace userManagement.Application.Services.Students;

public sealed class StudentService : IStudentService
{
    private readonly IStudentsRepository _students;
    private readonly ICurrentUserService _current;

    public StudentService(IStudentsRepository students, ICurrentUserService current)
    {
        _students = students;
        _current = current;
    }

    private void EnsureAuthenticated()
    {
        if (_current.UserId is null)
            throw new UnauthorizedAccessException("Authentication required.");
    }

    public async Task<int> CreateAsync(CreateStudentDto input)
    {
        EnsureAuthenticated();

        var entity = new Student
        {
            FirstName = input.FirstName.Trim(),
            LastName = input.LastName.Trim(),
            BirthDate = input.BirthDate
        };

        await _students.AddAsync(entity);
        return entity.Id;
    }

    public async Task<IEnumerable<StudentDto>> GetAllAsync()
    {
        EnsureAuthenticated();

        var list = await _students.GetAllStudents();
        return list.Select(MapToDto);
    }

    public async Task<StudentDto> GetByIdAsync(int id)
    {
        EnsureAuthenticated();

        var s = await _students.GetStudentById(id);
        if (s is null) throw new KeyNotFoundException("Student not found.");

        return MapToDto(s);
    }

    public async Task UpdateAsync(int id, UpdateStudentDto input)
    {
        EnsureAuthenticated();

        var s = await _students.GetStudentById(id);
        if (s is null) throw new KeyNotFoundException("Student not found.");

        s.FirstName = input.FirstName.Trim();
        s.LastName = input.LastName.Trim();
        s.BirthDate = input.BirthDate;

        await _students.UpdateAsync(s);
    }

    public async Task DeleteAsync(int id)
    {
        EnsureAuthenticated();

        await _students.DeleteAsync(id);
    }

    private static StudentDto MapToDto(Student s) => new()
    {
        Id = s.Id,
        FirstName = s.FirstName,
        LastName = s.LastName,
        BirthDate = s.BirthDate
    };
}
