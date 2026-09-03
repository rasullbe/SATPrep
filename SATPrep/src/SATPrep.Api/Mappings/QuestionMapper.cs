using System;
using System.Linq;
using SATPrep.Api.Entities;
using SATPrep.Api.DTOs;

namespace SATPrep.Api.Mappings;

public static class QuestionMapper
{
    public static Question ToEntity(this QuestionCreateDto dto)
    {
        if (dto is null) throw new ArgumentNullException(nameof(dto));

        var question = new Question
        {
            TopicId = dto.TopicId,
            Text = dto.Text?.Trim() ?? string.Empty,
            Difficulty = dto.Difficulty,
            Choices = dto.Choices?.Select(c => c.ToEntity()).ToList() ?? new System.Collections.Generic.List<Choice>(),
            QuestionTags = dto.TagIds?.Select(tid => new QuestionTag { TagId = tid }).ToList() ?? new System.Collections.Generic.List<QuestionTag>()
        };

        // Ensure reverse navigation for choices
        foreach (var choice in question.Choices)
            choice.Question = question;

        foreach (var qt in question.QuestionTags)
            qt.Question = question;

        return question;
    }

    public static QuestionGetDto ToGetDto(this Question entity)
    {
        if (entity is null) throw new ArgumentNullException(nameof(entity));

        var choices = entity.Choices?.Select(c => c.ToGetDto()) ?? System.Array.Empty<ChoiceGetDto>();
        var tags = entity.QuestionTags?.Select(qt => qt.Tag.ToGetDto()) ?? System.Array.Empty<TagGetDto>();

        return new QuestionGetDto(entity.QuestionId, entity.TopicId, entity.Text ?? string.Empty, entity.Difficulty, choices, tags);
    }

    public static QuestionAdminDto ToAdminDto(this Question entity)
    {
        if (entity is null) throw new ArgumentNullException(nameof(entity));

        var choices = entity.Choices?.Select(c => c.ToAdminDto()) ?? System.Array.Empty<ChoiceAdminDto>();
        var tags = entity.QuestionTags?.Select(qt => qt.Tag.ToGetDto()) ?? System.Array.Empty<TagGetDto>();

        return new QuestionAdminDto(entity.QuestionId, entity.TopicId, entity.Text ?? string.Empty, entity.Difficulty, choices, tags);
    }

    public static Question UpdateFrom(this Question entity, QuestionUpdateDto dto)
    {
        if (entity is null) throw new ArgumentNullException(nameof(entity));
        if (dto is null) throw new ArgumentNullException(nameof(dto));

        if (dto.Text is not null)
            entity.Text = dto.Text.Trim();

        if (dto.Difficulty is not null)
            entity.Difficulty = dto.Difficulty.Value;

        if (dto.Choices is not null)
        {
            // Replace choices: for simplicity, clear and add new ones
            entity.Choices.Clear();
            foreach (var c in dto.Choices)
            {
                var choice = c.ToEntity();
                choice.Question = entity;
                entity.Choices.Add(choice);
            }
        }

        if (dto.TagIds is not null)
        {
            entity.QuestionTags.Clear();
            foreach (var tid in dto.TagIds)
            {
                var qt = new QuestionTag { TagId = tid, Question = entity };
                entity.QuestionTags.Add(qt);
            }
        }

        return entity;
    }
}
