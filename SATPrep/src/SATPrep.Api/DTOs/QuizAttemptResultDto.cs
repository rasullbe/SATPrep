using System;
using System.Collections.Generic;

namespace SATPrep.Api.DTOs;

public record QuizAttemptResultDto(
    long QuizAttemptId,
    long QuizId,
    string QuizTitle,
    int Score,
    int PointsCorrect,
    int TotalQuestions,
    int? TimeTakenSeconds,
    DateTime StartedAt,
    DateTime CompletedAt,
    IEnumerable<QuestionResultDto> Questions,
    IEnumerable<SectionScoreDto> SectionBreakdown);

public record QuestionResultDto(
    long QuestionId,
    string Text,
    string Difficulty,
    long TopicId,
    string TopicName,
    string SubjectName,
    IEnumerable<ChoiceGetDto> Choices,
    long? SelectedChoiceId,
    long? CorrectChoiceId,
    bool IsCorrect);

public record SectionScoreDto(
    string SubjectName,
    IEnumerable<ScoreDetailDto> Sections);

public record ScoreDetailDto(
    string Name,
    int Correct,
    int Total,
    int Percentage);

public record UserMistakeDto(
    long QuestionId,
    string Text,
    string Difficulty,
    long TopicId,
    string TopicName,
    string SubjectName,
    IEnumerable<ChoiceGetDto> Choices,
    long? SelectedChoiceId,
    long? CorrectChoiceId,
    DateTime AnsweredAt,
    long QuizId,
    string QuizTitle);