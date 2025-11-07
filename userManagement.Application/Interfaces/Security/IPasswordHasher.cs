namespace userManagement.Application.Interfaces.Security;

public interface IPasswordHasher
{
    string Hash(string plainPassword);
    bool Verify(string hash, string hashedPassword);
}