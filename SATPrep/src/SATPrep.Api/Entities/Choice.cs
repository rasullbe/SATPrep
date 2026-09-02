using System.ComponentModel.DataAnnotations;

namespace SATPrep.Api.Entities;

public class Choice
{
    public long ChoiceId { get; set; }

    public long QuestionId { get; set; }
    public Question Question { get; set; } = null!;

    [Required]
    public string Text { get; set; } = string.Empty;

    public bool IsCorrect { get; set; } = false;
}
