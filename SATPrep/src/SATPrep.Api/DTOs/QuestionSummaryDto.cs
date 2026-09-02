using SATPrep.Api.Entities;

namespace SATPrep.Api.DTOs;

// Public question summary (no correct answers exposed)
public record QuestionSummaryDto(long QuestionId, string Text, Difficulty Difficulty);
