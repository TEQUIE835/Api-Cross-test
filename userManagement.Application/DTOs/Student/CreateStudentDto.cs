namespace userManagement.Application.DTOs.Students;

public sealed class CreateStudentDto
{
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
    public DateOnly BirthDate { get; init; }
}