using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ThiExam.Infrastructure.Data;

namespace ThiExam.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoriesController : ControllerBase
{
    private readonly ExamSystemContext _context;

    public CategoriesController(ExamSystemContext context)
    {
        _context = context;
    }

    public class CategoryDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public int? ParentId { get; set; }
        public int QuestionCount { get; set; }
    }

    /// <summary>
    /// Lấy danh sách danh mục
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<List<CategoryDto>>> GetCategories([FromQuery] int? teacherId)
    {
        var query = _context.Categories
            .Include(c => c.Questions)
            .AsNoTracking()
            .AsQueryable();

        // Filter by teacher (include categories with no teacher assigned)
        if (teacherId.HasValue)
        {
            query = query.Where(c => c.TeacherId == null || c.TeacherId == teacherId);
        }

        var categories = await query
            .OrderBy(c => c.Name)
            .Select(c => new CategoryDto
            {
                Id = c.Id,
                Name = c.Name,
                ParentId = c.ParentId,
                QuestionCount = c.Questions.Count
            })
            .ToListAsync();

        return Ok(categories);
    }

    /// <summary>
    /// Lấy chi tiết danh mục
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<CategoryDto>> GetCategory(int id)
    {
        var category = await _context.Categories
            .Include(c => c.Questions)
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == id);

        if (category == null)
            return NotFound(new { message = "Không tìm thấy danh mục" });

        return Ok(new CategoryDto
        {
            Id = category.Id,
            Name = category.Name,
            ParentId = category.ParentId,
            QuestionCount = category.Questions.Count
        });
    }
}
