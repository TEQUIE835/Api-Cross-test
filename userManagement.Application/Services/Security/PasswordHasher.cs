using userManagement.Application.Interfaces.Security;

namespace userManagement.Application.Services.Security;

public class PasswordHasher : IPasswordHasher
{
    public string Hash(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password);
    }

    public bool Verify(string hash, string hashedPassword)
    {
        return BCrypt.Net.BCrypt.Verify(hash, hashedPassword);
    }
}