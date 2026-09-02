using System;

namespace SATPrep.Api.DTOs;

public record AnswerResultGetDto(long QuestionId, long? SelectedChoiceId, bool IsCorrect, DateTime AnsweredAt);
