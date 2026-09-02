using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SATPrep.Api.Entities;

public class Topic
{
    public long TopicId { get; set; }
    public long SubjectId { get; set; }
    public Subject Subject { get; set; } = null!;

    [Required, MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    public ICollection<Question> Questions { get; set; } = new List<Question>();
}
