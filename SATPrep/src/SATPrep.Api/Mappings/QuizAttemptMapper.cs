using System;
using System.Linq;
using SATPrep.Api.Entities;
using SATPrep.Api.DTOs;

namespace SATPrep.Api.Mappings;

public static class QuizAttemptMapper
{
    public static QuizAttemptGetDto ToGetDto(this QuizAttempt entity)
    {
        if (entity is null) throw new ArgumentNullException(nameof(entity));

        var answers = entity.Answers?.Select(a => new AnswerResultGetDto(a.QuestionId, a.SelectedChoiceId, a.IsCorrect, a.AnsweredAt))
            ?? Array.Empty<AnswerResultGetDto>();

        return new QuizAttemptGetDto(
            entity.QuizAttemptId,
            entity.QuizId,
            entity.UserId,
            entity.StartedAt,
            entity.CompletedAt,
            entity.Score,
            entity.PointsCorrect,
            entity.TotalQuestions,
            entity.TimeTakenSeconds,
            answers);
    }
}