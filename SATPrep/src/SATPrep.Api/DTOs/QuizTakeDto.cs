using System;
using System.Collections.Generic;

namespace SATPrep.Api.DTOs;

public record QuizTakeDto(
    long QuizId,
    string Title,
    string Description,
    int? TimeLimitMinutes,
    int QuestionCount,
    IEnumerable<TakeQuestionDto> Questions);

public record TakeQuestionDto(
    long QuestionId,
    string Text,
    string Difficulty,
    IEnumerable<ChoiceGetDto> Choices);