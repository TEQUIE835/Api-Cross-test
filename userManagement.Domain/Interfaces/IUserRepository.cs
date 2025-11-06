using userManagement.Domain.Entities;

namespace userManagement.Domain.Interfaces;

public interface IUserRepository
{
    Task<User?> GetUserById(string username);
    Task RegisterUser(User user);
    Task<IEnumerable<User>> GetAllUsers();
    Task<User> GetUserById(int id);
    Task UpdateUser(User user);
    Task DeleteUser(int id);
}