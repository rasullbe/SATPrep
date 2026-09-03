using System;
using SATPrep.Api.Entities;
using SATPrep.Api.DTOs;

namespace SATPrep.Api.Mappings;

public static class ChoiceMapper
{
    public static Choice ToEntity(this ChoiceCreateDto dto)
    {
        if (dto is null) throw new ArgumentNullException(nameof(dto));

        return new Choice
        {
            Text = dto.Text?.Trim() ?? string.Empty,
            IsCorrect = dto.IsCorrect
        };
    }

    public static ChoiceGetDto ToGetDto(this Choice entity)
    {
        if (entity is null) throw new ArgumentNullException(nameof(entity));

        return new ChoiceGetDto(entity.ChoiceId, entity.Text ?? string.Empty);
    }

    public static ChoiceAdminDto ToAdminDto(this Choice entity)
    {
        if (entity is null) throw new ArgumentNullException(nameof(entity));

        return new ChoiceAdminDto(entity.ChoiceId, entity.Text ?? string.Empty, entity.IsCorrect);
    }
}
