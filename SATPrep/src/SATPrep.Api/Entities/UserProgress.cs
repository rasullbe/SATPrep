namespace SATPrep.Api.Entities;

public class UserProgress
{
    public long UserProgressId { get; set; }

    public long UserId { get; set; }
    public User User { get; set; } = null!;

    public long TopicId { get; set; }
    public Topic Topic { get; set; } = null!;

    public int QuestionsAttempted { get; set; }
    public int CorrectAnswers { get; set; }
}
