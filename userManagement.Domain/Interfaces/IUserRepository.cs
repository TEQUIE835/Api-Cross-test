using userManagement.Domain.Entities;

namespace userManagement.Domain.Interfaces;

public interface IUserRepository
{
    // Buscar por username
    Task<User?> GetUserByUsername(string username);

    // Buscar por email
    Task<User?> GetUserByEmail(string email);

    // Registrar usuario
    Task RegisterUser(User user);

    // Obtener todos
    Task<IEnumerable<User>> GetAllUsers();

    // Buscar por Id (cuando viene de controller:)
    Task<User> GetUserById(int id);

    // Actualizar usuario
    Task UpdateUser(User user);

    // Eliminar usuario
    Task DeleteUser(int id);
}