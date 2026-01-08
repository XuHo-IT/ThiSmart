using System;
using System.Collections.Generic;

namespace ThiSmartUI.Models;

public partial class Question
{
    public int Id { get; set; }

    public int? CategoryId { get; set; }

    public int? TeacherId { get; set; }

    public string Content { get; set; } = null!;

    public string? ImageUrl { get; set; }

    public string? Level { get; set; }

    public string? QuestionType { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual Category? Category { get; set; }

    public virtual ICollection<ExamQuestion> ExamQuestions { get; set; } = new List<ExamQuestion>();

    public virtual ICollection<QuestionOption> QuestionOptions { get; set; } = new List<QuestionOption>();

    public virtual ICollection<StudentAnswer> StudentAnswers { get; set; } = new List<StudentAnswer>();

    public virtual User? Teacher { get; set; }
}
