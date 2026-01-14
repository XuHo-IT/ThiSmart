using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http.Json;
using System.Text.Json;

namespace ThiExam.Web.Pages.Teacher.Questions
{
    public class IndexModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<IndexModel> _logger;

        public IndexModel(IHttpClientFactory httpClientFactory, ILogger<IndexModel> logger)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        // Filter parameters
        [BindProperty(SupportsGet = true)]
        public string? SearchTerm { get; set; }

        [BindProperty(SupportsGet = true)]
        public int? CategoryId { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? Level { get; set; }

        [BindProperty(SupportsGet = true)]
        public int PageNumber { get; set; } = 1;

        public int PageSize { get; set; } = 10;

        // Data
        public List<QuestionListItem> Questions { get; set; } = new();
        public List<CategoryDto> Categories { get; set; } = new();

        // Statistics
        public int TotalQuestions { get; set; }
        public int TotalPages { get; set; }
        public int EasyCount { get; set; }
        public int MediumCount { get; set; }
        public int HardCount { get; set; }

        // Messages
        [TempData]
        public string? SuccessMessage { get; set; }

        [TempData]
        public string? ErrorMessage { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            // Check authentication
            var userId = HttpContext.Session.GetInt32("UserId");
            var role = HttpContext.Session.GetString("Role");

            if (userId == null || role != "Teacher")
            {
                return RedirectToPage("/Account/Login");
            }

            try
            {
                var client = _httpClientFactory.CreateClient("ThiExamAPI");

                // Load categories
                var categoriesResponse = await client.GetAsync($"api/categories?teacherId={userId}");
                if (categoriesResponse.IsSuccessStatusCode)
                {
                    Categories = await categoriesResponse.Content.ReadFromJsonAsync<List<CategoryDto>>() ?? new();
                }

                // Build query string for questions
                var queryParams = new List<string>
                {
                    $"teacherId={userId}",
                    $"pageNumber={PageNumber}",
                    $"pageSize={PageSize}"
                };

                if (!string.IsNullOrEmpty(SearchTerm))
                    queryParams.Add($"search={Uri.EscapeDataString(SearchTerm)}");
                
                if (CategoryId.HasValue)
                    queryParams.Add($"categoryId={CategoryId}");
                
                if (!string.IsNullOrEmpty(Level))
                    queryParams.Add($"level={Uri.EscapeDataString(Level)}");

                var queryString = string.Join("&", queryParams);

                // Load questions
                var questionsResponse = await client.GetAsync($"api/questions?{queryString}");
                if (questionsResponse.IsSuccessStatusCode)
                {
                    var result = await questionsResponse.Content.ReadFromJsonAsync<QuestionListResponse>();
                    if (result != null)
                    {
                        Questions = result.Items;
                        TotalQuestions = result.TotalCount;
                        TotalPages = result.TotalPages;
                        EasyCount = result.EasyCount;
                        MediumCount = result.MediumCount;
                        HardCount = result.HardCount;
                    }
                }
                else
                {
                    _logger.LogWarning("Failed to load questions: {StatusCode}", questionsResponse.StatusCode);
                    ErrorMessage = "Không thể tải danh sách câu hỏi.";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading questions");
                ErrorMessage = "Đã xảy ra lỗi khi tải dữ liệu.";
            }

            return Page();
        }

        public async Task<IActionResult> OnPostDeleteAsync(int questionId)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToPage("/Account/Login");
            }

            try
            {
                var client = _httpClientFactory.CreateClient("ThiExamAPI");
                var response = await client.DeleteAsync($"api/questions/{questionId}");

                if (response.IsSuccessStatusCode)
                {
                    SuccessMessage = "Đã xóa câu hỏi thành công!";
                }
                else
                {
                    var error = await response.Content.ReadAsStringAsync();
                    ErrorMessage = $"Không thể xóa câu hỏi: {error}";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting question {QuestionId}", questionId);
                ErrorMessage = "Đã xảy ra lỗi khi xóa câu hỏi.";
            }

            return RedirectToPage();
        }
    }

    // DTOs for API responses
    public class QuestionListItem
    {
        public int Id { get; set; }
        public string Content { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
        public int? CategoryId { get; set; }
        public string? CategoryName { get; set; }
        public string? Level { get; set; }
        public string? QuestionType { get; set; }
        public DateTime? CreatedAt { get; set; }
        public int OptionsCount { get; set; }
    }

    public class QuestionListResponse
    {
        public List<QuestionListItem> Items { get; set; } = new();
        public int TotalCount { get; set; }
        public int TotalPages { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int EasyCount { get; set; }
        public int MediumCount { get; set; }
        public int HardCount { get; set; }
    }

    public class CategoryDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}
