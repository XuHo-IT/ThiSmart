using System;
using System.Collections.Generic;

namespace ThiExam.Core.Entities;

public partial class ExamSession
{
    public int Id { get; set; }

    public int? ExamId { get; set; }

    public string? AccessCode { get; set; }

    public DateTime? StartTime { get; set; }

    public DateTime? EndTime { get; set; }

    public bool? IsActive { get; set; }

    public virtual Exam? Exam { get; set; }

    public virtual ICollection<StudentAttempt> StudentAttempts { get; set; } = new List<StudentAttempt>();
}
