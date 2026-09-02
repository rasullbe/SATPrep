using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SATPrep.Api.Entities;

public class Subject
{
    public long SubjectId { get; set; }

    [Required, MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    public ICollection<Topic> Topics { get; set; } = new List<Topic>();
}
