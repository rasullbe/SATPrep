namespace SATPrep.Api.Entities;

public class QuizQuestion
{
    public long Id { get; set; }

    public long QuizId { get; set; }
    public Quiz Quiz { get; set; } = null!;

    public long QuestionId { get; set; }
    public Question Question { get; set; } = null!;

    public int Order { get; set; }
}