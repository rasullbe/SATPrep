using System.Collections.Generic;
using SATPrep.Api.Entities;

namespace SATPrep.Api.DTOs;

// Full detail for public consumption - choices do not include correctness
public record QuestionGetDto(long QuestionId, long TopicId, string Text, Difficulty Difficulty, IEnumerable<ChoiceGetDto> Choices, IEnumerable<TagGetDto> Tags);
