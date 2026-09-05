using System.ComponentModel.DataAnnotations;

namespace SATPrep.Api.DTOs;

public class QuizUpdateDto
{
    [MaxLength(200)]
    public string? Title { get; set; }

    public string? Description { get; set; }
}
