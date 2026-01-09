using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ThiExam.Common.DTOs;
using ThiExam.Core.Entities;
using ThiExam.Infrastructure.Data;

namespace ThiExam.Web.Pages.Account
{
    public class RegisterModel : PageModel
    {
        private readonly ExamSystemContext _context;
        private readonly ILogger<RegisterModel> _logger;

        public RegisterModel(ExamSystemContext context, ILogger<RegisterModel> logger)
        {
            _context = context;
            _logger = logger;
        }

        [BindProperty]
        public RegisterViewModel Input { get; set; } = new();

        public void OnGet()
        {
            if (HttpContext.Session.GetInt32("UserId") != null)
            {
                Response.Redirect("/Index");
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                // Check if user already exists
                if (await _context.Users.AnyAsync(u => u.Username.ToLower() == Input.Username.ToLower()))
                {
                    ModelState.AddModelError("Input.Username", "Tên đăng nhập đã tồn tại");
                    return Page();
                }

                if (await _context.Users.AnyAsync(u => u.Email.ToLower() == Input.Email.ToLower()))
                {
                    ModelState.AddModelError("Input.Email", "Email đã tồn tại");
                    return Page();
                }

                var studentRole = await _context.Roles.FirstOrDefaultAsync(r => r.RoleName == "Student");

                var user = new User
                {
                    Username = Input.Username,
                    Email = Input.Email,
                    FullName = Input.FullName,
                    PasswordHash = Input.Password, // TODO: Hash password in production
                    RoleId = studentRole?.Id,
                    CreatedAt = DateTime.UtcNow
                };

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                _logger.LogInformation("User '{Username}' registered successfully", user.Username);

                // Auto login after registration
                HttpContext.Session.SetInt32("UserId", user.Id);
                HttpContext.Session.SetString("Username", user.Username);
                HttpContext.Session.SetString("FullName", user.FullName ?? user.Username);
                HttpContext.Session.SetString("Role", studentRole?.RoleName ?? "Student");

                return RedirectToPage("/Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during registration for user '{Username}'", Input.Username);
                ModelState.AddModelError(string.Empty, "Đã xảy ra lỗi. Vui lòng thử lại sau.");
                return Page();
            }
        }
    }
}
