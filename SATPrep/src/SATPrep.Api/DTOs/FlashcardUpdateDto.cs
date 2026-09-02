using System;

namespace SATPrep.Api.DTOs;

public class FlashcardUpdateDto
{
    public string? Front { get; set; }
    public string? Back { get; set; }
    public DateTime? NextReview { get; set; }
}
