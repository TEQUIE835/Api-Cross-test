using userManagement.Application.DTOs.Students;

namespace userManagement.Application.Interfaces.Students;

public interface IStudentService
{
    Task<int> CreateAsync(CreateStudentDto input);
    Task<IEnumerable<StudentDto>> GetAllAsync();
    Task<StudentDto> GetByIdAsync(int id);
    Task UpdateAsync(int id, UpdateStudentDto input);
    Task DeleteAsync(int id);
}