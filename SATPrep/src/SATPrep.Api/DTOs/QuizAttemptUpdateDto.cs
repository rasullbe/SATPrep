using System.ComponentModel.DataAnnotations;

namespace SATPrep.Api.DTOs;

public class QuizAttemptUpdateDto
{
    public List<AnswerCreateDto>? Answers { get; set; }
}
