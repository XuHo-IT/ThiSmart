using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ThiExam.Common.DTOs;
using ThiExam.Core.Entities;
using ThiExam.Infrastructure.Data;

namespace ThiExam.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly ExamSystemContext _context;

    public AuthController(ExamSystemContext context)
    {
        _context = context;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (await _context.Users.AnyAsync(u => u.Username.ToLower() == model.Username.ToLower()))
        {
            return BadRequest(new { message = "Tên đăng nhập đã tồn tại" });
        }

        if (await _context.Users.AnyAsync(u => u.Email.ToLower() == model.Email.ToLower()))
        {
            return BadRequest(new { message = "Email đã tồn tại" });
        }

        var studentRole = await _context.Roles.FirstOrDefaultAsync(r => r.RoleName == "Student");

        var user = new User
        {
            Username = model.Username,
            Email = model.Email,
            FullName = model.FullName,
            PasswordHash = model.Password, // TODO: Hash password in production
            RoleId = studentRole?.Id,
            CreatedAt = DateTime.UtcNow
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return Ok(new UserResponse
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email,
            FullName = user.FullName,
            RoleName = studentRole?.RoleName
        });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        var user = await _context.Users
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.Username.ToLower() == model.Username.ToLower());

        if (user == null || user.PasswordHash != model.Password)
        {
            return Unauthorized(new { message = "Tên đăng nhập hoặc mật khẩu không chính xác" });
        }

        return Ok(new UserResponse
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email,
            FullName = user.FullName,
            RoleName = user.Role?.RoleName
        });
    }
}

