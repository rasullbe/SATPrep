using System;
using System.Linq;
using SATPrep.Api.Entities;
using SATPrep.Api.DTOs;

namespace SATPrep.Api.Mappings;

public static class QuizMapper
{
    public static Quiz ToEntity(this QuizCreateDto dto)
    {
        if (dto is null) throw new ArgumentNullException(nameof(dto));

        var quiz = new Quiz
        {
            Title = dto.Title?.Trim() ?? string.Empty,
            Description = dto.Description ?? string.Empty,
            CreatedById = dto.CreatedById,
            CreatedAt = DateTime.UtcNow,
            QuizQuestions = dto.QuestionIds?.Select((qid, idx) => new QuizQuestion { QuestionId = qid, Order = idx }).ToList() ?? new System.Collections.Generic.List<QuizQuestion>()
        };

        foreach (var qq in quiz.QuizQuestions)
            qq.Quiz = quiz;

        return quiz;
    }

    public static QuizGetDto ToGetDto(this Quiz entity)
    {
        if (entity is null) throw new ArgumentNullException(nameof(entity));

        return new QuizGetDto(entity.QuizId, entity.Title ?? string.Empty, entity.Description ?? string.Empty, entity.CreatedById, entity.CreatedAt);
    }

    public static QuizDetailGetDto ToDetailGetDto(this Quiz entity)
    {
        if (entity is null) throw new ArgumentNullException(nameof(entity));

        var questions = entity.QuizQuestions?.Select(qq => qq.Question).Where(q => q != null)
            .Select(q => new QuestionSummaryDto(q.QuestionId, q.Text ?? string.Empty, q.Difficulty)) ?? System.Array.Empty<QuestionSummaryDto>();

        return new QuizDetailGetDto(entity.QuizId, entity.Title ?? string.Empty, entity.Description ?? string.Empty, entity.CreatedById, entity.CreatedAt, questions);
    }
}
