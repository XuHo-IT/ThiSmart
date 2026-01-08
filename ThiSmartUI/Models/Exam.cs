using System;
using System.Collections.Generic;

namespace ThiSmartUI.Models;

public partial class Exam
{
    public int Id { get; set; }

    public int? TeacherId { get; set; }

    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    public int Duration { get; set; }

    public decimal? PassScore { get; set; }

    public bool? IsRandom { get; set; }

    public string? SnapshotUrl { get; set; }

    public string? AntiCheatSettings { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual ICollection<ExamQuestion> ExamQuestions { get; set; } = new List<ExamQuestion>();

    public virtual ICollection<ExamSession> ExamSessions { get; set; } = new List<ExamSession>();

    public virtual User? Teacher { get; set; }
}
