using System.ComponentModel.DataAnnotations;

namespace SATPrep.Api.DTOs;

public class ChoiceCreateDto
{
    [Required, MaxLength(1000)]
    public string Text { get; set; } = string.Empty;

    public bool IsCorrect { get; set; }
}
