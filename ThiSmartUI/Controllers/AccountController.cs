using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ThiSmartUI.Models;
using ThiSmartUI.Models.ViewModels;

namespace ThiSmartUI.Controllers;

/// <summary>
/// Controller xử lý Authentication - Login/Logout
/// Tuân thủ Single Responsibility Principle: Chỉ xử lý việc xác thực
/// </summary>
public class AccountController : Controller
{
    private readonly ExamSystemContext _context;
    private readonly ILogger<AccountController> _logger;

    // Constructor Injection - Best practice cho DI
    public AccountController(ExamSystemContext context, ILogger<AccountController> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// GET: /Account/Login
    /// Hiển thị form đăng nhập
    /// </summary>
    [HttpGet]
    public IActionResult Login(string? returnUrl = null)
    {
        // Nếu đã đăng nhập rồi -> redirect về Home
        if (HttpContext.Session.GetInt32("UserId") != null)
        {
            return RedirectToAction("Index", "Home");
        }

        // Lưu returnUrl để sau khi login redirect về trang ban đầu
        ViewData["ReturnUrl"] = returnUrl;
        return View();
    }

    /// <summary>
    /// POST: /Account/Login
    /// Xử lý đăng nhập với validation
    /// </summary>
    /// <remarks>
    /// Security Notes:
    /// - So sánh password bằng hash (đang dùng plain compare tạm thời)
    /// - Server-side validation bắt buộc, không chỉ dựa vào client
    /// - Logging failed attempts để monitor brute-force
    /// </remarks>
    [HttpPost]
    [ValidateAntiForgeryToken] // Chống CSRF attack
    public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;

        // Server-side validation - KHÔNG BAO GIỜ tin tưởng client
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        try
        {
            // Tìm user theo username (case-insensitive)
            // AsNoTracking() vì chỉ đọc, không cần track changes -> better performance
            var user = await _context.Users
                .AsNoTracking()
                .Include(u => u.Role) // Eager loading Role để dùng sau
                .FirstOrDefaultAsync(u => u.Username.ToLower() == model.Username.ToLower());

            if (user == null)
            {
                // Log failed attempt (security monitoring)
                _logger.LogWarning("Login failed: User '{Username}' not found", model.Username);

                // Thông báo chung chung để tránh enumeration attack
                ModelState.AddModelError(string.Empty, "Tên đăng nhập hoặc mật khẩu không đúng");
                return View(model);
            }

            // So sánh password
            // TODO: Trong production, sử dụng BCrypt.Verify() thay vì so sánh trực tiếp
            // Hiện tại database có thể đang lưu plain text hoặc hash
            if (!VerifyPassword(model.Password, user.PasswordHash))
            {
                _logger.LogWarning("Login failed: Invalid password for user '{Username}'", model.Username);
                ModelState.AddModelError(string.Empty, "Tên đăng nhập hoặc mật khẩu không đúng");
                return View(model);
            }

            // Đăng nhập thành công - Lưu thông tin vào Session
            HttpContext.Session.SetInt32("UserId", user.Id);
            HttpContext.Session.SetString("Username", user.Username);
            HttpContext.Session.SetString("FullName", user.FullName ?? user.Username);
            HttpContext.Session.SetString("Role", user.Role?.RoleName ?? "Student");

            _logger.LogInformation("User '{Username}' logged in successfully", user.Username);

            // Redirect về returnUrl hoặc Home
            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return RedirectToAction("Index", "Home");
        }
        catch (Exception ex)
        {
            // Global exception handling - Log và thông báo user-friendly
            _logger.LogError(ex, "Error during login for user '{Username}'", model.Username);
            ModelState.AddModelError(string.Empty, "Đã xảy ra lỗi. Vui lòng thử lại sau.");
            return View(model);
        }
    }

    /// <summary>
    /// POST: /Account/Logout
    /// Đăng xuất - Clear session
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Logout()
    {
        var username = HttpContext.Session.GetString("Username");

        // Clear toàn bộ session
        HttpContext.Session.Clear();

        _logger.LogInformation("User '{Username}' logged out", username);

        return RedirectToAction("Login");
    }

    /// <summary>
    /// Verify password - So sánh password nhập vào với hash trong DB
    /// </summary>
    /// <remarks>
    /// TODO: Production cần dùng BCrypt hoặc Argon2 để hash password
    /// Hiện tại đang so sánh trực tiếp vì DB có thể chưa hash
    /// </remarks>
    private static bool VerifyPassword(string inputPassword, string storedHash)
    {
        // Tạm thời so sánh trực tiếp
        // Production: return BCrypt.Net.BCrypt.Verify(inputPassword, storedHash);
        return inputPassword == storedHash;
    }
}
