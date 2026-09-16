using System;
using System.Linq;
using System.Collections.Generic;
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
            TimeLimitMinutes = dto.TimeLimitMinutes,
            ShuffleQuestions = dto.ShuffleQuestions,
            IsPublished = dto.IsPublished,
            QuizQuestions = dto.QuestionIds?.Select((qid, idx) => new QuizQuestion { QuestionId = qid, Order = idx }).ToList() ?? new List<QuizQuestion>()
        };

        foreach (var qq in quiz.QuizQuestions)
            qq.Quiz = quiz;

        return quiz;
    }

    public static QuizGetDto ToGetDto(this Quiz entity)
    {
        if (entity is null) throw new ArgumentNullException(nameof(entity));

        return new QuizGetDto(
            entity.QuizId,
            entity.Title ?? string.Empty,
            entity.Description ?? string.Empty,
            entity.CreatedById,
            entity.CreatedAt,
            entity.TimeLimitMinutes,
            entity.ShuffleQuestions,
            entity.IsPublished,
            entity.QuizQuestions?.Count ?? 0);
    }

    public static QuizDetailGetDto ToDetailGetDto(this Quiz entity)
    {
        if (entity is null) throw new ArgumentNullException(nameof(entity));

        var questions = entity.QuizQuestions?.Select(qq => qq.Question).Where(q => q != null)
            .Select(q => new QuestionSummaryDto(q.QuestionId, q.Text ?? string.Empty, q.Difficulty))
            ?? Array.Empty<QuestionSummaryDto>();

        return new QuizDetailGetDto(
            entity.QuizId,
            entity.Title ?? string.Empty,
            entity.Description ?? string.Empty,
            entity.CreatedById,
            entity.CreatedAt,
            entity.TimeLimitMinutes,
            entity.ShuffleQuestions,
            entity.IsPublished,
            questions);
    }

    public static QuizTakeDto ToTakeDto(this Quiz entity)
    {
        if (entity is null) throw new ArgumentNullException(nameof(entity));

        var questions = entity.QuizQuestions?
            .OrderBy(qq => qq.Order)
            .Select(qq => qq.Question)
            .Where(q => q != null)
            .Select(q => new TakeQuestionDto(
                q.QuestionId,
                q.Text ?? string.Empty,
                q.Difficulty.ToString(),
                q.Choices?.Select(c => c.ToGetDto()) ?? Array.Empty<ChoiceGetDto>()))
            ?? Array.Empty<TakeQuestionDto>();

        return new QuizTakeDto(
            entity.QuizId,
            entity.Title ?? string.Empty,
            entity.Description ?? string.Empty,
            entity.TimeLimitMinutes,
            entity.QuizQuestions?.Count ?? 0,
            questions);
    }

    public static void UpdateFrom(this Quiz entity, QuizUpdateDto dto)
    {
        if (entity is null) throw new ArgumentNullException(nameof(entity));
        if (dto is null) throw new ArgumentNullException(nameof(dto));

        if (dto.Title is not null)
            entity.Title = dto.Title.Trim();

        if (dto.Description is not null)
            entity.Description = dto.Description;

        if (dto.TimeLimitMinutes is not null)
            entity.TimeLimitMinutes = dto.TimeLimitMinutes;

        if (dto.ShuffleQuestions is not null)
            entity.ShuffleQuestions = dto.ShuffleQuestions.Value;

        if (dto.IsPublished is not null)
            entity.IsPublished = dto.IsPublished.Value;
    }
}