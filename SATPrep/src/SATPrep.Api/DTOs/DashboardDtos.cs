using System;
using System.Collections.Generic;

namespace SATPrep.Api.DTOs;

public record DashboardDto(
    UserStatsDto User,
    IEnumerable<QuizGetDto> AvailableQuizzes,
    IEnumerable<QuizAttemptGetDto> RecentAttempts,
    IEnumerable<TopicAccuracyDto> WeakAreas);

public record UserStatsDto(
    long UserId,
    string Name,
    int StudyStreak,
    int TotalStudyMinutes,
    int TotalSessions,
    int TotalQuestionsAnswered,
    DateTime? LastStudiedAt,
    int AverageScore);

public record TopicAccuracyDto(
    long TopicId,
    string TopicName,
    long SubjectId,
    string SubjectName,
    int QuestionsAttempted,
    int CorrectAnswers,
    int AccuracyPercentage);