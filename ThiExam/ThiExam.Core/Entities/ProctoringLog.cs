using System;
using System.Collections.Generic;

namespace ThiExam.Core.Entities;

public partial class ProctoringLog
{
    public int Id { get; set; }

    public int? AttemptId { get; set; }

    public string? EventType { get; set; }

    public DateTime? LogTime { get; set; }

    public string? Details { get; set; }

    public virtual StudentAttempt? Attempt { get; set; }
}
