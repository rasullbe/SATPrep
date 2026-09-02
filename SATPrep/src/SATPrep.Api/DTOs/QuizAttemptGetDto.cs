using System;
using System.Collections.Generic;

namespace SATPrep.Api.DTOs;

public record QuizAttemptGetDto(long QuizAttemptId, long QuizId, long UserId, DateTime StartedAt, DateTime? CompletedAt, int Score, IEnumerable<AnswerResultGetDto> Answers);
