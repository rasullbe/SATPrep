using System.Collections.Generic;
using SATPrep.Api.Entities;

namespace SATPrep.Api.DTOs;

public class QuestionUpdateDto
{
    public string? Text { get; set; }
    public Difficulty? Difficulty { get; set; }
    public List<ChoiceCreateDto>? Choices { get; set; }
    public List<long>? TagIds { get; set; }
}
