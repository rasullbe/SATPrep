using System;
using SATPrep.Api.Entities;
using SATPrep.Api.DTOs;
using System.Linq;

namespace SATPrep.Api.Mappings;

public static class TopicMapper
{
    public static Topic ToEntity(this TopicCreateDto dto)
    {
        if (dto is null) throw new ArgumentNullException(nameof(dto));

        return new Topic
        {
            SubjectId = dto.SubjectId,
            Name = dto.Name?.Trim() ?? string.Empty,
            Questions = new System.Collections.Generic.List<Question>()
        };
    }

    public static TopicGetDto ToGetDto(this Topic entity)
    {
        if (entity is null) throw new ArgumentNullException(nameof(entity));

        return new TopicGetDto(entity.TopicId, entity.SubjectId, entity.Name ?? string.Empty);
    }
}
