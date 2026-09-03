using System;
using System.Linq;
using SATPrep.Api.Entities;
using SATPrep.Api.DTOs;

namespace SATPrep.Api.Mappings;

public static class QuizAttemptMapper
{
    public static QuizAttempt ToEntity(this QuizAttemptCreateDto dto)
    {
        if (dto is null) throw new ArgumentNullException(nameof(dto));

        var attempt = new QuizAttempt
        {
            QuizId = dto.QuizId,
            UserId = dto.UserId,
            StartedAt = DateTime.UtcNow,
            CompletedAt = null,
            Score = 0,
            Answers = dto.Answers?.Select(a => new Answer
            {
                QuestionId = a.QuestionId,
                SelectedChoiceId = a.SelectedChoiceId,
                IsCorrect = false,
                AnsweredAt = DateTime.UtcNow
            }).ToList() ?? new System.Collections.Generic.List<Answer>()
        };

        foreach (var ans in attempt.Answers)
            ans.QuizAttempt = attempt;

        return attempt;
    }

    public static QuizAttemptGetDto ToGetDto(this QuizAttempt entity)
    {
        if (entity is null) throw new ArgumentNullException(nameof(entity));

        var answers = entity.Answers?.Select(a => new AnswerResultGetDto(a.QuestionId, a.SelectedChoiceId, a.IsCorrect, a.AnsweredAt)) ?? System.Array.Empty<AnswerResultGetDto>();

        return new QuizAttemptGetDto(entity.QuizAttemptId, entity.QuizId, entity.UserId, entity.StartedAt, entity.CompletedAt, entity.Score, answers);
    }
}
