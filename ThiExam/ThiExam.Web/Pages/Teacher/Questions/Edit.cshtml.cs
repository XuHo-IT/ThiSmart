using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Net.Http.Json;

namespace ThiExam.Web.Pages.Teacher.Questions
{
    public class EditModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<EditModel> _logger;

        public EditModel(IHttpClientFactory httpClientFactory, ILogger<EditModel> logger)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        [BindProperty]
        public QuestionEditInput Input { get; set; } = new();

        [BindProperty]
        public List<OptionInput> Options { get; set; } = new();

        public List<CategoryDto> Categories { get; set; } = new();

        public DateTime CreatedAt { get; set; }

        [TempData]
        public string? SuccessMessage { get; set; }

        [TempData]
        public string? ErrorMessage { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            var role = HttpContext.Session.GetString("Role");

            if (userId == null || role != "Teacher")
            {
                return RedirectToPage("/Account/Login");
            }

            try
            {
                var client = _httpClientFactory.CreateClient("ThiExamAPI");

                // Load question
                var questionResponse = await client.GetAsync($"api/questions/{id}");
                if (!questionResponse.IsSuccessStatusCode)
                {
                    ErrorMessage = "Không tìm thấy câu hỏi.";
                    return RedirectToPage("/Teacher/Questions/Index");
                }

                var question = await questionResponse.Content.ReadFromJsonAsync<QuestionDetailDto>();
                if (question == null || question.TeacherId != userId)
                {
                    ErrorMessage = "Bạn không có quyền chỉnh sửa câu hỏi này.";
                    return RedirectToPage("/Teacher/Questions/Index");
                }

                // Map to input
                Input = new QuestionEditInput
                {
                    Id = question.Id,
                    Content = question.Content,
                    ImageUrl = question.ImageUrl,
                    CategoryId = question.CategoryId,
                    Level = question.Level ?? "Easy",
                    QuestionType = question.QuestionType ?? "MultipleChoice"
                };

                Options = question.Options?.Select(o => new OptionInput
                {
                    Content = o.Content,
                    IsCorrect = o.IsCorrect
                }).ToList() ?? new();

                CreatedAt = question.CreatedAt ?? DateTime.Now;

                // Load categories
                await LoadCategoriesAsync(userId.Value);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading question {QuestionId}", id);
                ErrorMessage = "Đã xảy ra lỗi khi tải câu hỏi.";
                return RedirectToPage("/Teacher/Questions/Index");
            }

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

                var request = new UpdateQuestionRequest
                {
                    Content = Input.Content,
                    ImageUrl = Input.ImageUrl,
                    CategoryId = Input.CategoryId,
                    Level = Input.Level,
                    Options = Input.QuestionType == "MultipleChoice"
                        ? Options.Where(o => !string.IsNullOrWhiteSpace(o.Content))
                                 .Select(o => new UpdateOptionRequest
                                 {
                                     Content = o.Content,
                                     IsCorrect = o.IsCorrect
                                 }).ToList()
                        : new List<UpdateOptionRequest>()
                };

                var response = await client.PutAsJsonAsync($"api/questions/{Input.Id}", request);

                if (response.IsSuccessStatusCode)
                {
                    SuccessMessage = "Cập nhật câu hỏi thành công!";
                    return RedirectToPage("/Teacher/Questions/Index");
                }
                else
                {
                    var error = await response.Content.ReadAsStringAsync();
                    _logger.LogWarning("Failed to update question: {Error}", error);
                    ErrorMessage = "Không thể cập nhật câu hỏi. Vui lòng thử lại.";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating question {QuestionId}", Input.Id);
                ErrorMessage = "Đã xảy ra lỗi khi cập nhật câu hỏi.";
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

    public class QuestionEditInput
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Nội dung câu hỏi là bắt buộc")]
        public string Content { get; set; } = string.Empty;

        [Url(ErrorMessage = "URL hình ảnh không hợp lệ")]
        public string? ImageUrl { get; set; }

        public int? CategoryId { get; set; }

        public string Level { get; set; } = "Easy";

        public string QuestionType { get; set; } = "MultipleChoice";
    }

    public class QuestionDetailDto
    {
        public int Id { get; set; }
        public string Content { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
        public int? CategoryId { get; set; }
        public string? CategoryName { get; set; }
        public int TeacherId { get; set; }
        public string? Level { get; set; }
        public string? QuestionType { get; set; }
        public DateTime? CreatedAt { get; set; }
        public List<OptionDetailDto>? Options { get; set; }
    }

    public class OptionDetailDto
    {
        public int Id { get; set; }
        public string Content { get; set; } = string.Empty;
        public bool IsCorrect { get; set; }
    }

    public class UpdateQuestionRequest
    {
        public string Content { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
        public int? CategoryId { get; set; }
        public string Level { get; set; } = "Easy";
        public List<UpdateOptionRequest> Options { get; set; } = new();
    }

    public class UpdateOptionRequest
    {
        public string Content { get; set; } = string.Empty;
        public bool IsCorrect { get; set; }
    }
}
