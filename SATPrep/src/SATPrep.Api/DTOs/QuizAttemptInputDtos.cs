using System.Collections.Generic;

namespace SATPrep.Api.DTOs;

public class CreateQuizAttemptDto
{
    public long QuizId { get; set; }
}

public class CompleteQuizAttemptDto
{
    public int? TimeTakenSeconds { get; set; }

    public List<AnswerCreateDto> Answers { get; set; } = new();
}

public class AnswerCreateDto
{
    public long QuestionId { get; set; }

    public long? SelectedChoiceId { get; set; }
}