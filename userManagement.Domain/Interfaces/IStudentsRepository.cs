using userManagement.Domain.Entities;

namespace userManagement.Domain.Interfaces;

public interface IStudentsRepository
{
    Task<IEnumerable<Student>> GetAllStudents();
    Task<Student> GetStudentById(int id);
    Task AddAsync(Student student);
    Task UpdateAsync(Student student);
    Task DeleteAsync(int id);
}