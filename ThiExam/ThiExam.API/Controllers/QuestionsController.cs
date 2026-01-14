using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ThiExam.Core.Entities;
using ThiExam.Infrastructure.Data;

namespace ThiExam.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class QuestionsController : ControllerBase
{
    private readonly ExamSystemContext _context;
    private readonly ILogger<QuestionsController> _logger;

    public QuestionsController(ExamSystemContext context, ILogger<QuestionsController> logger)
    {
        _context = context;
        _logger = logger;
    }

    #region DTOs
    
    public class QuestionDto
    {
        public int Id { get; set; }
        public string Content { get; set; } = null!;
        public string? ImageUrl { get; set; }
        public int? CategoryId { get; set; }
        public string? CategoryName { get; set; }
        public string? Level { get; set; }
        public string? QuestionType { get; set; }
        public DateTime? CreatedAt { get; set; }
        public List<QuestionOptionDto> Options { get; set; } = new();
    }

    public class QuestionOptionDto
    {
        public int Id { get; set; }
        public string Content { get; set; } = null!;
        public bool IsCorrect { get; set; }
    }

    public class CreateQuestionRequest
    {
        public string Content { get; set; } = null!;
        public string? ImageUrl { get; set; }
        public int? CategoryId { get; set; }
        public int TeacherId { get; set; }
        public string Level { get; set; } = "Medium";
        public string QuestionType { get; set; } = "MultipleChoice";
        public List<CreateOptionRequest> Options { get; set; } = new();
    }

    public class CreateOptionRequest
    {
        public string Content { get; set; } = null!;
        public bool IsCorrect { get; set; }
    }

    public class UpdateQuestionRequest
    {
        public string Content { get; set; } = null!;
        public string? ImageUrl { get; set; }
        public int? CategoryId { get; set; }
        public string Level { get; set; } = "Medium";
        public string QuestionType { get; set; } = "MultipleChoice";
        public List<CreateOptionRequest> Options { get; set; } = new();
    }

    public class QuestionListResponse
    {
        public List<QuestionDto> Items { get; set; } = new();
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public int EasyCount { get; set; }
        public int MediumCount { get; set; }
        public int HardCount { get; set; }
    }

    #endregion

    /// <summary>
    /// Lấy danh sách câu hỏi với phân trang và filter
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<QuestionListResponse>> GetQuestions(
        [FromQuery] int? teacherId,
        [FromQuery] int? categoryId,
        [FromQuery] string? level,
        [FromQuery] string? search,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        var query = _context.Questions
            .Include(q => q.Category)
            .Include(q => q.QuestionOptions)
            .AsNoTracking()
            .AsQueryable();

        // Base filter for teacher
        var baseQuery = query;
        if (teacherId.HasValue)
            baseQuery = baseQuery.Where(q => q.TeacherId == teacherId);

        // Get statistics before applying other filters
        var easyCount = await baseQuery.CountAsync(q => q.Level == "Easy" || q.Level == "1");
        var mediumCount = await baseQuery.CountAsync(q => q.Level == "Medium" || q.Level == "2");
        var hardCount = await baseQuery.CountAsync(q => q.Level == "Hard" || q.Level == "3");

        // Apply filters
        if (teacherId.HasValue)
            query = query.Where(q => q.TeacherId == teacherId);

        if (categoryId.HasValue)
            query = query.Where(q => q.CategoryId == categoryId);

        if (!string.IsNullOrEmpty(level))
            query = query.Where(q => q.Level == level);

        if (!string.IsNullOrEmpty(search))
            query = query.Where(q => q.Content.Contains(search));

        // Get total count
        var totalCount = await query.CountAsync();

        // Apply pagination
        var items = await query
            .OrderByDescending(q => q.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(q => new QuestionDto
            {
                Id = q.Id,
                Content = q.Content,
                ImageUrl = q.ImageUrl,
                CategoryId = q.CategoryId,
                CategoryName = q.Category != null ? q.Category.Name : null,
                Level = q.Level,
                QuestionType = q.QuestionType,
                CreatedAt = q.CreatedAt,
                Options = q.QuestionOptions.Select(o => new QuestionOptionDto
                {
                    Id = o.Id,
                    Content = o.Content,
                    IsCorrect = o.IsCorrect ?? false
                }).ToList()
            })
            .ToListAsync();

        return Ok(new QuestionListResponse
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize),
            EasyCount = easyCount,
            MediumCount = mediumCount,
            HardCount = hardCount
        });
    }

    /// <summary>
    /// Lấy chi tiết một câu hỏi
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<QuestionDto>> GetQuestion(int id)
    {
        var question = await _context.Questions
            .Include(q => q.Category)
            .Include(q => q.QuestionOptions)
            .AsNoTracking()
            .FirstOrDefaultAsync(q => q.Id == id);

        if (question == null)
            return NotFound(new { message = "Không tìm thấy câu hỏi" });

        return Ok(new QuestionDto
        {
            Id = question.Id,
            Content = question.Content,
            ImageUrl = question.ImageUrl,
            CategoryId = question.CategoryId,
            CategoryName = question.Category?.Name,
            Level = question.Level,
            QuestionType = question.QuestionType,
            CreatedAt = question.CreatedAt,
            Options = question.QuestionOptions.Select(o => new QuestionOptionDto
            {
                Id = o.Id,
                Content = o.Content,
                IsCorrect = o.IsCorrect ?? false
            }).ToList()
        });
    }

    /// <summary>
    /// Tạo câu hỏi mới
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<QuestionDto>> CreateQuestion([FromBody] CreateQuestionRequest request)
    {
        // Validation
        if (string.IsNullOrWhiteSpace(request.Content))
            return BadRequest(new { message = "Nội dung câu hỏi không được để trống" });

        var validOptions = request.Options.Where(o => !string.IsNullOrWhiteSpace(o.Content)).ToList();
        if (validOptions.Count < 2)
            return BadRequest(new { message = "Cần có ít nhất 2 đáp án" });

        if (!validOptions.Any(o => o.IsCorrect))
            return BadRequest(new { message = "Cần có ít nhất 1 đáp án đúng" });

        // Create question
        var question = new Question
        {
            Content = request.Content.Trim(),
            ImageUrl = string.IsNullOrWhiteSpace(request.ImageUrl) ? null : request.ImageUrl.Trim(),
            CategoryId = request.CategoryId,
            TeacherId = request.TeacherId,
            Level = request.Level,
            QuestionType = request.QuestionType,
            CreatedAt = DateTime.UtcNow
        };

        _context.Questions.Add(question);
        await _context.SaveChangesAsync();

        // Create options
        foreach (var optionRequest in validOptions)
        {
            var option = new QuestionOption
            {
                QuestionId = question.Id,
                Content = optionRequest.Content.Trim(),
                IsCorrect = optionRequest.IsCorrect
            };
            _context.QuestionOptions.Add(option);
        }

        await _context.SaveChangesAsync();

        _logger.LogInformation("Question {QuestionId} created by teacher {TeacherId}", question.Id, request.TeacherId);

        // Return created question
        return CreatedAtAction(nameof(GetQuestion), new { id = question.Id }, new QuestionDto
        {
            Id = question.Id,
            Content = question.Content,
            ImageUrl = question.ImageUrl,
            CategoryId = question.CategoryId,
            Level = question.Level,
            QuestionType = question.QuestionType,
            CreatedAt = question.CreatedAt,
            Options = validOptions.Select(o => new QuestionOptionDto
            {
                Content = o.Content,
                IsCorrect = o.IsCorrect
            }).ToList()
        });
    }

    /// <summary>
    /// Cập nhật câu hỏi
    /// </summary>
    [HttpPut("{id}")]
    public async Task<ActionResult<QuestionDto>> UpdateQuestion(int id, [FromBody] UpdateQuestionRequest request)
    {
        var question = await _context.Questions
            .Include(q => q.QuestionOptions)
            .FirstOrDefaultAsync(q => q.Id == id);

        if (question == null)
            return NotFound(new { message = "Không tìm thấy câu hỏi" });

        // Validation
        if (string.IsNullOrWhiteSpace(request.Content))
            return BadRequest(new { message = "Nội dung câu hỏi không được để trống" });

        var validOptions = request.Options.Where(o => !string.IsNullOrWhiteSpace(o.Content)).ToList();
        if (validOptions.Count < 2)
            return BadRequest(new { message = "Cần có ít nhất 2 đáp án" });

        if (!validOptions.Any(o => o.IsCorrect))
            return BadRequest(new { message = "Cần có ít nhất 1 đáp án đúng" });

        // Update question
        question.Content = request.Content.Trim();
        question.ImageUrl = string.IsNullOrWhiteSpace(request.ImageUrl) ? null : request.ImageUrl.Trim();
        question.CategoryId = request.CategoryId;
        question.Level = request.Level;
        question.QuestionType = request.QuestionType;

        // Remove old options and add new ones
        _context.QuestionOptions.RemoveRange(question.QuestionOptions);

        foreach (var optionRequest in validOptions)
        {
            var option = new QuestionOption
            {
                QuestionId = question.Id,
                Content = optionRequest.Content.Trim(),
                IsCorrect = optionRequest.IsCorrect
            };
            _context.QuestionOptions.Add(option);
        }

        await _context.SaveChangesAsync();

        _logger.LogInformation("Question {QuestionId} updated", question.Id);

        return Ok(new QuestionDto
        {
            Id = question.Id,
            Content = question.Content,
            ImageUrl = question.ImageUrl,
            CategoryId = question.CategoryId,
            Level = question.Level,
            QuestionType = question.QuestionType,
            CreatedAt = question.CreatedAt,
            Options = validOptions.Select(o => new QuestionOptionDto
            {
                Content = o.Content,
                IsCorrect = o.IsCorrect
            }).ToList()
        });
    }

    /// <summary>
    /// Xóa câu hỏi
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteQuestion(int id)
    {
        var question = await _context.Questions
            .Include(q => q.QuestionOptions)
            .FirstOrDefaultAsync(q => q.Id == id);

        if (question == null)
            return NotFound(new { message = "Không tìm thấy câu hỏi" });

        // Remove options first
        _context.QuestionOptions.RemoveRange(question.QuestionOptions);
        _context.Questions.Remove(question);

        await _context.SaveChangesAsync();

        _logger.LogInformation("Question {QuestionId} deleted", id);

        return NoContent();
    }


}
