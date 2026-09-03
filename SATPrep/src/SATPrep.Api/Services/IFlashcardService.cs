using SATPrep.Api.DTOs;

namespace SATPrep.Api.Services;

public interface IFlashcardService
{
    Task<FlashcardGetDto?> GetByIdAsync(long flashcardId);
    Task<List<FlashcardGetDto>> GetAllAsync();
    Task<List<FlashcardGetDto>> GetByUserAsync(long userId);
    Task<FlashcardGetDto?> CreateAsync(FlashcardCreateDto createDto);
    Task<FlashcardGetDto?> UpdateAsync(long flashcardId, FlashcardUpdateDto updateDto);
    Task<bool> DeleteAsync(long flashcardId);
    Task<bool> UpdateNextReviewAsync(long flashcardId, DateTime nextReview);
}
