using System.ComponentModel.DataAnnotations;

namespace SATPrep.Api.DTOs;

public class TopicCreateDto
{
    [Required]
    public long SubjectId { get; set; }

    [Required, MaxLength(200)]
    public string Name { get; set; } = string.Empty;
}
