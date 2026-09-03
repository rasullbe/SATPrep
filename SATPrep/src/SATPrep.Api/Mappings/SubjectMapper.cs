using System;
using SATPrep.Api.Entities;
using SATPrep.Api.DTOs;

namespace SATPrep.Api.Mappings;

public static class SubjectMapper
{
    public static Subject ToEntity(this SubjectCreateDto dto)
    {
        if (dto is null) throw new ArgumentNullException(nameof(dto));

        return new Subject
        {
            Name = dto.Name?.Trim() ?? string.Empty,
            Topics = new System.Collections.Generic.List<Topic>()
        };
    }

    public static SubjectGetDto ToGetDto(this Subject entity)
    {
        if (entity is null) throw new ArgumentNullException(nameof(entity));

        return new SubjectGetDto(entity.SubjectId, entity.Name ?? string.Empty, entity.Topics?.Select(t => t.ToGetDto()) ?? System.Array.Empty<TopicGetDto>());
    }
}
