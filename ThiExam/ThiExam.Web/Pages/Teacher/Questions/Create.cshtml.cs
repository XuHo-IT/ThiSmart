using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Net.Http.Json;

namespace ThiExam.Web.Pages.Teacher.Questions
{
    public class CreateModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<CreateModel> _logger;

        public CreateModel(IHttpClientFactory httpClientFactory, ILogger<CreateModel> logger)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        [BindProperty]
        public QuestionInput Input { get; set; } = new();

        [BindProperty]
        public List<OptionInput> Options { get; set; } = new();

        public List<CategoryDto> Categories { get; set; } = new();

        [TempData]
        public string? SuccessMessage { get; set; }

        [TempData]
        public string? ErrorMessage { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            var role = HttpContext.Session.GetString("Role");

            if (userId == null || role != "Teacher")
            {
                return RedirectToPage("/Account/Login");
            }

            await LoadCategoriesAsync(userId.Value);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            var role = HttpContext.Session.GetString("Role");

            if (userId == null || role != "Teacher")
            {
                return RedirectToPage("/Account/Login");
            }

            // Validate
            if (string.IsNullOrWhiteSpace(Input.Content))
            {
                ModelState.AddModelError("Input.Content", "Nội dung câu hỏi là bắt buộc.");
            }

            // For multiple choice questions, validate options
            if (Input.QuestionType == "MultipleChoice")
            {
                var validOptions = Options.Where(o => !string.IsNullOrWhiteSpace(o.Content)).ToList();
                if (validOptions.Count < 2)
                {
                    ModelState.AddModelError("", "Câu trắc nghiệm cần ít nhất 2 lựa chọn.");
                }

                var correctCount = validOptions.Count(o => o.IsCorrect);
                if (correctCount != 1)
                {
                    ModelState.AddModelError("", "Phải chọn đúng 1 đáp án đúng.");
                }
            }

            if (!ModelState.IsValid)
            {
                await LoadCategoriesAsync(userId.Value);
                return Page();
            }

            try
            {
                var client = _httpClientFactory.CreateClient("ThiExamAPI");

                var request = new CreateQuestionRequest
                {
                    Content = Input.Content,
                    ImageUrl = Input.ImageUrl,
                    CategoryId = Input.CategoryId,
                    TeacherId = userId.Value,
                    Level = Input.Level,
                    QuestionType = Input.QuestionType,
                    Options = Input.QuestionType == "MultipleChoice"
                        ? Options.Where(o => !string.IsNullOrWhiteSpace(o.Content))
                                 .Select(o => new CreateOptionRequest
                                 {
                                     Content = o.Content,
                                     IsCorrect = o.IsCorrect
                                 }).ToList()
                        : new List<CreateOptionRequest>()
                };

                var response = await client.PostAsJsonAsync("api/questions", request);

                if (response.IsSuccessStatusCode)
                {
                    SuccessMessage = "Tạo câu hỏi thành công!";
                    return RedirectToPage("/Teacher/Questions/Index");
                }
                else
                {
                    var error = await response.Content.ReadAsStringAsync();
                    _logger.LogWarning("Failed to create question: {Error}", error);
                    ErrorMessage = "Không thể tạo câu hỏi. Vui lòng thử lại.";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating question");
                ErrorMessage = "Đã xảy ra lỗi khi tạo câu hỏi.";
            }

            await LoadCategoriesAsync(userId.Value);
            return Page();
        }

        private async Task LoadCategoriesAsync(int teacherId)
        {
            try
            {
                var client = _httpClientFactory.CreateClient("ThiExamAPI");
                var response = await client.GetAsync($"api/categories?teacherId={teacherId}");

                if (response.IsSuccessStatusCode)
                {
                    Categories = await response.Content.ReadFromJsonAsync<List<CategoryDto>>() ?? new();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading categories");
            }
        }
    }

    public class QuestionInput
    {
        [Required(ErrorMessage = "Nội dung câu hỏi là bắt buộc")]
        public string Content { get; set; } = string.Empty;

        [Url(ErrorMessage = "URL hình ảnh không hợp lệ")]
        public string? ImageUrl { get; set; }

        public int? CategoryId { get; set; }

        public string Level { get; set; } = "Easy";

        public string QuestionType { get; set; } = "MultipleChoice";
    }

    public class OptionInput
    {
        public string Content { get; set; } = string.Empty;
        public bool IsCorrect { get; set; }
    }

    public class CreateQuestionRequest
    {
        public string Content { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
        public int? CategoryId { get; set; }
        public int TeacherId { get; set; }
        public string Level { get; set; } = "Easy";
        public string QuestionType { get; set; } = "MultipleChoice";
        public List<CreateOptionRequest> Options { get; set; } = new();
    }

    public class CreateOptionRequest
    {
        public string Content { get; set; } = string.Empty;
        public bool IsCorrect { get; set; }
    }
}
