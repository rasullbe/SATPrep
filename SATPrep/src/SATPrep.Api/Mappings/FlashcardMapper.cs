using System;
using SATPrep.Api.Entities;
using SATPrep.Api.DTOs;

namespace SATPrep.Api.Mappings;

public static class FlashcardMapper
{
    public static Flashcard ToEntity(this FlashcardCreateDto dto)
    {
        if (dto is null) throw new ArgumentNullException(nameof(dto));

        return new Flashcard
        {
            UserId = dto.UserId,
            Front = dto.Front?.Trim() ?? string.Empty,
            Back = dto.Back?.Trim() ?? string.Empty,
            CreatedAt = DateTime.UtcNow
        };
    }

    public static FlashcardGetDto ToGetDto(this Flashcard entity)
    {
        if (entity is null) throw new ArgumentNullException(nameof(entity));

        return new FlashcardGetDto(entity.FlashcardId, entity.UserId, entity.Front ?? string.Empty, entity.Back ?? string.Empty, entity.CreatedAt, entity.NextReview);
    }

    public static Flashcard UpdateFrom(this Flashcard entity, FlashcardUpdateDto dto)
    {
        if (entity is null) throw new ArgumentNullException(nameof(entity));
        if (dto is null) throw new ArgumentNullException(nameof(dto));

        if (dto.Front is not null)
            entity.Front = dto.Front.Trim();

        if (dto.Back is not null)
            entity.Back = dto.Back.Trim();

        if (dto.NextReview is not null)
            entity.NextReview = dto.NextReview;

        return entity;
    }
}
