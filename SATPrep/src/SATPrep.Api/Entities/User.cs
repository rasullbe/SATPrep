using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SATPrep.Api.Entities;

public class User
{
    public long UserId { get; set; }

    [Required, MaxLength(100)]
    public string Name { get; set; }

    [Required, MaxLength(200)]
    public string Email { get; set; } = string.Empty;

    [Required, MaxLength(200)]
    public string Password { get; set; } = string.Empty;

    [Required]
    public Role Role { get; set; } = Role.User;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation
    public ICollection<QuizAttempt> Attempts { get; set; } = new List<QuizAttempt>();
    public ICollection<Quiz> CreatedQuizzes { get; set; } = new List<Quiz>();
    public ICollection<Flashcard> Flashcards { get; set; } = new List<Flashcard>();
}
