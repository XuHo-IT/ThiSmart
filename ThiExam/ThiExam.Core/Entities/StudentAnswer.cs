using System;
using System.Collections.Generic;

namespace ThiExam.Core.Entities;

public partial class StudentAnswer
{
    public int Id { get; set; }

    public int AttemptId { get; set; }

    public int? QuestionId { get; set; }

    public int? SelectedOptionId { get; set; }

    public string? EssayAnswer { get; set; }

    public bool? IsFlagged { get; set; }

    public virtual StudentAttempt Attempt { get; set; } = null!;

    public virtual Question? Question { get; set; }

    public virtual QuestionOption? SelectedOption { get; set; }
}
