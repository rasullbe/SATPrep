using System.ComponentModel.DataAnnotations;

namespace SATPrep.Api.DTOs;

public class FlashcardCreateDto
{
    [Required]
    public long UserId { get; set; }

    [Required, MaxLength(1000)]
    public string Front { get; set; } = string.Empty;

    [Required, MaxLength(4000)]
    public string Back { get; set; } = string.Empty;
}
