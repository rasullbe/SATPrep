using System;

namespace SATPrep.Api.DTOs;

public record QuizGetDto(long QuizId, string Title, string Description, long CreatedById, DateTime CreatedAt);
