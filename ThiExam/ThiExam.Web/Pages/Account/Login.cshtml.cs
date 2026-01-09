using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ThiExam.Common.DTOs;
using ThiExam.Infrastructure.Data;

namespace ThiExam.Web.Pages.Account;

public class LoginModel : PageModel
{
    private readonly ExamSystemContext _context;
    private readonly ILogger<LoginModel> _logger;

    public LoginModel(ExamSystemContext context, ILogger<LoginModel> logger)
    {
        _context = context;
        _logger = logger;
    }

    [BindProperty]
    public LoginViewModel Input { get; set; } = new();

    [BindProperty(SupportsGet = true)]
    public string? ReturnUrl { get; set; }

    public void OnGet(string? returnUrl = null)
    {
        if (HttpContext.Session.GetInt32("UserId") != null)
        {
            Response.Redirect("/Index");
        }
        ReturnUrl = returnUrl;
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        try
        {
            var user = await _context.Users
                .AsNoTracking()
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Username.ToLower() == Input.Username.ToLower());

            if (user == null || !VerifyPassword(Input.Password, user.PasswordHash))
            {
                _logger.LogWarning("Login failed for user '{Username}'", Input.Username);
                ModelState.AddModelError(string.Empty, "Tên đăng nhập hoặc mật khẩu không đúng");
                return Page();
            }

            HttpContext.Session.SetInt32("UserId", user.Id);
            HttpContext.Session.SetString("Username", user.Username);
            HttpContext.Session.SetString("FullName", user.FullName ?? user.Username);
            HttpContext.Session.SetString("Role", user.Role?.RoleName ?? "Student");

            _logger.LogInformation("User '{Username}' logged in successfully", user.Username);

            if (!string.IsNullOrEmpty(ReturnUrl) && Url.IsLocalUrl(ReturnUrl))
            {
                return Redirect(ReturnUrl);
            }

            return RedirectToPage("/Index");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during login for user '{Username}'", Input.Username);
            ModelState.AddModelError(string.Empty, "Đã xảy ra lỗi. Vui lòng thử lại sau.");
            return Page();
        }
    }

    private static bool VerifyPassword(string inputPassword, string storedHash)
    {
        return inputPassword == storedHash;
    }
}
