using System.ComponentModel.DataAnnotations;

namespace ThiExam.Common.DTOs;

/// <summary>
/// ViewModel cho form đăng nhập
/// Sử dụng Data Annotations để validate input ở cả client-side và server-side
/// </summary>
public class LoginViewModel
{
    /// <summary>
    /// Tên đăng nhập - Required, tối thiểu 3 ký tự
    /// </summary>
    [Required(ErrorMessage = "Vui lòng nhập tên đăng nhập")]
    [MinLength(3, ErrorMessage = "Tên đăng nhập phải có ít nhất 3 ký tự")]
    [Display(Name = "Tên đăng nhập")]
    public string Username { get; set; } = string.Empty;

    /// <summary>
    /// Mật khẩu - Required, tối thiểu 6 ký tự
    /// </summary>
    [Required(ErrorMessage = "Vui lòng nhập mật khẩu")]
    [MinLength(6, ErrorMessage = "Mật khẩu phải có ít nhất 6 ký tự")]
    [DataType(DataType.Password)]
    [Display(Name = "Mật khẩu")]
    public string Password { get; set; } = string.Empty;

    /// <summary>
    /// Ghi nhớ đăng nhập - Mở rộng thời gian session
    /// </summary>
    [Display(Name = "Ghi nhớ đăng nhập")]
    public bool RememberMe { get; set; }
}
