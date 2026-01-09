namespace ThiExam.Common.DTOs;

public class UserResponse
{
    public int Id { get; set; }
    public string Username { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string? FullName { get; set; }
    public string? RoleName { get; set; }
}

