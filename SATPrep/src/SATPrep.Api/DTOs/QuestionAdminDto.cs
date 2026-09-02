using System.Collections.Generic;
using SATPrep.Api.Entities;

namespace SATPrep.Api.DTOs;

// Admin-facing question DTO includes correctness information
public record QuestionAdminDto(long QuestionId, long TopicId, string Text, Difficulty Difficulty, IEnumerable<ChoiceAdminDto> Choices, IEnumerable<TagGetDto> Tags);
