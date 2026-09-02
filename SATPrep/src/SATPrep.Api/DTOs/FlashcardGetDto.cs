using System;

namespace SATPrep.Api.DTOs;

public record FlashcardGetDto(long FlashcardId, long UserId, string Front, string Back, DateTime CreatedAt, DateTime? NextReview);
