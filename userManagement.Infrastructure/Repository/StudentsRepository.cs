using Microsoft.EntityFrameworkCore;
using userManagement.Domain.Entities;
using userManagement.Domain.Interfaces;
using userManagement.Infrastructure.Persistence;

namespace userManagement.Infrastructure.Repository;

public class StudentsRepository : IStudentsRepository
{
    private readonly AppDbContext _dbContext;

    public StudentsRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IEnumerable<Student>> GetAllStudents()
    {
        return await _dbContext.Students.ToListAsync();
    }

    public async Task<Student> GetStudentById(int id)
    {
        return await _dbContext.Students.FindAsync(id);
    }

    public async Task AddAsync(Student student)
    {
        _dbContext.Students.Add(student);
        await _dbContext.SaveChangesAsync();
    }

    public async Task UpdateAsync(Student student)
    {
        _dbContext.Students.Update(student);
        await _dbContext.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var student = await _dbContext.Students.FindAsync(id);
        _dbContext.Students.Remove(student);
        await _dbContext.SaveChangesAsync();
    }
}