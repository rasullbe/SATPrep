using System;
using System.Collections.Generic;

namespace SATPrep.Api.DTOs;

public record QuizGetDto(
    long QuizId,
    string Title,
    string Description,
    long CreatedById,
    DateTime CreatedAt,
    int? TimeLimitMinutes,
    bool ShuffleQuestions,
    bool IsPublished,
    int QuestionCount);