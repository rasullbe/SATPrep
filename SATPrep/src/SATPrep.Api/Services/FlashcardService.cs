using SATPrep.Api.DTOs;
using SATPrep.Api.Mappings;
using SATPrep.Api.Repositories;

namespace SATPrep.Api.Services;

public class FlashcardService : IFlashcardService
{
    private readonly IFlashcardRepository _flashcardRepository;

    public FlashcardService(IFlashcardRepository flashcardRepository)
    {
        _flashcardRepository = flashcardRepository;
    }

    public async Task<FlashcardGetDto?> GetByIdAsync(long flashcardId)
    {
        var flashcard = await _flashcardRepository.GetByIdAsync(flashcardId);
        return flashcard?.ToGetDto();
    }

    public async Task<List<FlashcardGetDto>> GetAllAsync()
    {
        var flashcards = await _flashcardRepository.GetAllAsync();
        return flashcards.Select(f => f.ToGetDto()).ToList();
    }

    public async Task<List<FlashcardGetDto>> GetByUserAsync(long userId)
    {
        var flashcards = await _flashcardRepository.GetAllAsync();
        return flashcards.Where(f => f.UserId == userId).Select(f => f.ToGetDto()).ToList();
    }

    public async Task<FlashcardGetDto?> CreateAsync(FlashcardCreateDto createDto)
    {
        if (createDto is null)
            throw new ArgumentNullException(nameof(createDto));

        var flashcard = createDto.ToEntity();
        await _flashcardRepository.AddAsync(flashcard);

        if (!await _flashcardRepository.SaveChangesAsync())
            return null;

        return flashcard.ToGetDto();
    }

    public async Task<FlashcardGetDto?> UpdateAsync(long flashcardId, FlashcardUpdateDto updateDto)
    {
        if (updateDto is null)
            throw new ArgumentNullException(nameof(updateDto));

        var flashcard = await _flashcardRepository.GetByIdAsync(flashcardId);
        if (flashcard is null)
            return null;

        flashcard.UpdateFrom(updateDto);

        if (!await _flashcardRepository.SaveChangesAsync())
            return null;

        return flashcard.ToGetDto();
    }

    public async Task<bool> DeleteAsync(long flashcardId)
    {
        var flashcard = await _flashcardRepository.GetByIdAsync(flashcardId);
        if (flashcard is null)
            return false;

        return await _flashcardRepository.SaveChangesAsync();
    }

    public async Task<bool> UpdateNextReviewAsync(long flashcardId, DateTime nextReview)
    {
        var flashcard = await _flashcardRepository.GetByIdAsync(flashcardId);
        if (flashcard is null)
            return false;

        flashcard.NextReview = nextReview;
        return await _flashcardRepository.SaveChangesAsync();
    }
}
