using System;

namespace SATPrep.Api.Entities;

public class Answer
{
    public long AnswerId { get; set; }

    public long QuizAttemptId { get; set; }
    public QuizAttempt QuizAttempt { get; set; } = null!;

    public long QuestionId { get; set; }
    public Question Question { get; set; } = null!;

    public long? SelectedChoiceId { get; set; }
    public Choice? SelectedChoice { get; set; }

    public bool IsCorrect { get; set; }

    public DateTime AnsweredAt { get; set; } = DateTime.UtcNow;
}
