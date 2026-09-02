using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SATPrep.Api.Entities;

public class Question
{
    public long QuestionId { get; set; }

    public long TopicId { get; set; }
    public Topic Topic { get; set; } = null!;

    [Required]
    public string Text { get; set; } = string.Empty;

    public Difficulty Difficulty { get; set; } = Difficulty.Medium;

    public ICollection<Choice> Choices { get; set; } = new List<Choice>();

    public ICollection<QuestionTag> QuestionTags { get; set; } = new List<QuestionTag>();
}
