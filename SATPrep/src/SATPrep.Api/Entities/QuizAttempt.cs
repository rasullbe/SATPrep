using System;
using System.Collections.Generic;

namespace SATPrep.Api.Entities;

public class QuizAttempt
{
    public long QuizAttemptId { get; set; }

    public long QuizId { get; set; }
    public Quiz Quiz { get; set; } = null!;

    public long UserId { get; set; }
    public User User { get; set; } = null!;

    public DateTime StartedAt { get; set; } = DateTime.UtcNow;
    public DateTime? CompletedAt { get; set; }

    public AttemptStatus Status { get; set; } = AttemptStatus.InProgress;

    public int Score { get; set; }

    public int PointsCorrect { get; set; }
    public int TotalQuestions { get; set; }

    public int? TimeTakenSeconds { get; set; }

    public ICollection<Answer> Answers { get; set; } = new List<Answer>();
}