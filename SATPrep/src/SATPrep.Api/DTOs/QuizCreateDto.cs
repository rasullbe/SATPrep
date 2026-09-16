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

    public int? TimeLimitMinutes { get; set; }

    public bool ShuffleQuestions { get; set; }

    public bool IsPublished { get; set; } = true;

    public List<long> QuestionIds { get; set; } = new();
}