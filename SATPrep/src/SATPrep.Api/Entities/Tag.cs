using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SATPrep.Api.Entities;

public class Tag
{
    public long TagId { get; set; }

    [Required, MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    public ICollection<QuestionTag> QuestionTags { get; set; } = new List<QuestionTag>();
}
