using System;
using System.ComponentModel.DataAnnotations;

namespace SATPrep.Api.Entities;

public class Flashcard
{
    public long FlashcardId { get; set; }

    public long UserId { get; set; }
    public User User { get; set; } = null!;

    [Required]
    public string Front { get; set; } = string.Empty;

    [Required]
    public string Back { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Optional field for spaced repetition
    public DateTime? NextReview { get; set; }
}
