using System;
using SATPrep.Api.Entities;
using SATPrep.Api.DTOs;

namespace SATPrep.Api.Mappings;

public static class TagMapper
{
    public static Tag ToEntity(this TagCreateDto dto)
    {
        if (dto is null) throw new ArgumentNullException(nameof(dto));

        return new Tag
        {
            Name = dto.Name?.Trim() ?? string.Empty,
            QuestionTags = new System.Collections.Generic.List<QuestionTag>()
        };
    }

    public static TagGetDto ToGetDto(this Tag entity)
    {
        if (entity is null) throw new ArgumentNullException(nameof(entity));

        return new TagGetDto(entity.TagId, entity.Name ?? string.Empty);
    }
}
