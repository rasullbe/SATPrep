using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using SATPrep.Api.Entities;

namespace SATPrep.Api.DTOs;

public class QuestionCreateDto
{
    [Required]
    public long TopicId { get; set; }

    [Required]
    public string Text { get; set; } = string.Empty;

    public Difficulty Difficulty { get; set; } = Difficulty.Medium;

    [Required]
    public List<ChoiceCreateDto> Choices { get; set; } = new List<ChoiceCreateDto>();

    public List<long> TagIds { get; set; } = new List<long>();
}
