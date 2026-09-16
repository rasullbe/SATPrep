using System;

namespace SATPrep.Api.DTOs;

public record UserGetDto(
    long UserId,
    string Name,
    string Email,
    string Role,
    DateTime CreatedAt,
    int StudyStreak,
    int TotalStudyMinutes,
    DateTime? LastStudiedAt);