using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SATPrep.Api.DTOs;

public class QuizCreateDto
{
    [Required, MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    public string? Description { get; set; }

    [Required]
    public long CreatedById { get; set; }

    // Either supply explicit question ids for an existing pool, or create questions separately
    public List<long> QuestionIds { get; set; } = new List<long>();
}
