using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SATPrep.Api.DTOs;

public class QuizAttemptCreateDto
{
    [Required]
    public long QuizId { get; set; }

    [Required]
    public long UserId { get; set; }

    // Answers supplied on submission
    public List<AnswerCreateDto> Answers { get; set; } = new List<AnswerCreateDto>();
}

public class AnswerCreateDto
{
    [Required]
    public long QuestionId { get; set; }

    public long? SelectedChoiceId { get; set; }
}
