using SATPrep.Api.Entities;

namespace SATPrep.Api.Repositories;

public interface IFlashcardRepository
{
    Task<Flashcard?> GetByIdAsync(long id);
    Task<List<Flashcard>> GetAllAsync();
    Task<List<Flashcard>> GetByUserAsync(long userId);
    Task AddAsync(Flashcard flashcard);
    void Remove(Flashcard flashcard);
    Task<bool> SaveChangesAsync();
}