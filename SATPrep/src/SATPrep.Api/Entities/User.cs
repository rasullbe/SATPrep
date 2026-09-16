using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SATPrep.Api.Entities;

public class User
{
    public long UserId { get; set; }

    [Required, MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required, MaxLength(200)]
    public string Email { get; set; } = string.Empty;

    [Required, MaxLength(300)]
    public string Password { get; set; } = string.Empty;

    [Required]
    public Role Role { get; set; } = Role.User;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public int StudyStreak { get; set; }
    public DateTime? LastStudiedAt { get; set; }
    public int TotalStudyMinutes { get; set; }

    // Navigation
    public ICollection<QuizAttempt> Attempts { get; set; } = new List<QuizAttempt>();
    public ICollection<Quiz> CreatedQuizzes { get; set; } = new List<Quiz>();
    public ICollection<Flashcard> Flashcards { get; set; } = new List<Flashcard>();
    public ICollection<StudySession> StudySessions { get; set; } = new List<StudySession>();
    public ICollection<UserProgress> Progress { get; set; } = new List<UserProgress>();
    public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
}