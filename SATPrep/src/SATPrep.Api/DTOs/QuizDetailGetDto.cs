using System;
using System.Collections.Generic;

namespace SATPrep.Api.DTOs;

public record QuizDetailGetDto(long QuizId, string Title, string Description, long CreatedById, DateTime CreatedAt, IEnumerable<QuestionSummaryDto> Questions);
