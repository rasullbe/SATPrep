using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SATPrep.Api.Entities;

public class Quiz
{
    public long QuizId { get; set; }

    [Required, MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public long CreatedById { get; set; }
    public User CreatedBy { get; set; } = null!;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<QuizQuestion> QuizQuestions { get; set; } = new List<QuizQuestion>();
}
