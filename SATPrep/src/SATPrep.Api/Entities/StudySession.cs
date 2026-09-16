using System;

namespace SATPrep.Api.Entities;

public class StudySession
{
    public long StudySessionId { get; set; }

    public long UserId { get; set; }
    public User User { get; set; } = null!;

    public DateTime Date { get; set; } = DateTime.UtcNow.Date;

    public int MinutesStudied { get; set; }
    public int QuestionsAnswered { get; set; }
    public int CorrectAnswers { get; set; }
}
