using SATPrep.Api.Entities;

public class QuestionTag
{
    public long Id { get; set; }
    public long QuestionId { get; set; }
    public Question Question { get; set; } = null!;
    public long TagId { get; set; }
    public Tag Tag { get; set; } = null!;
}