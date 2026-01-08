using System;
using System.Collections.Generic;

namespace ThiSmartUI.Models;

public partial class StudentAttempt
{
    public int Id { get; set; }

    public int? SessionId { get; set; }

    public int? StudentId { get; set; }

    public DateTime? StartedAt { get; set; }

    public DateTime? SubmittedAt { get; set; }

    public decimal? TotalScore { get; set; }

    public string? Status { get; set; }

    public virtual ICollection<ProctoringLog> ProctoringLogs { get; set; } = new List<ProctoringLog>();

    public virtual ExamSession? Session { get; set; }

    public virtual User? Student { get; set; }

    public virtual ICollection<StudentAnswer> StudentAnswers { get; set; } = new List<StudentAnswer>();
}
