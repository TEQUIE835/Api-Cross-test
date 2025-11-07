using userManagement.Application.DTOs.Users;

namespace userManagement.Application.Interfaces.Users;

public interface IUserService
{
    Task<IEnumerable<UserDto>> GetAllAsync();
    Task<UserDto> GetByIdAsync(int id);
    Task<int> CreateAsync(CreateUserDto input);
    Task UpdateAsync(int id, UpdateUserDto input);
    Task DeleteAsync(int id);
}