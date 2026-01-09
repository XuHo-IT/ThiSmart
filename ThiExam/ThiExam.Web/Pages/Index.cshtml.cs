using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ThiExam.Web.Pages;

public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;

    public IndexModel(ILogger<IndexModel> logger)
    {
        _logger = logger;
    }

    public string? FullName { get; set; }
    public string? Username { get; set; }
    public bool IsLoggedIn { get; set; }

    public void OnGet()
    {
        FullName = HttpContext.Session.GetString("FullName");
        Username = HttpContext.Session.GetString("Username");
        IsLoggedIn = HttpContext.Session.GetInt32("UserId") != null;
    }

    public IActionResult OnPostLogout()
    {
        HttpContext.Session.Clear();
        return RedirectToPage("/Index");
    }
}

