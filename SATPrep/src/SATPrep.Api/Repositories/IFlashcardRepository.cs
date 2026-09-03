using SATPrep.Api.Entities;

namespace SATPrep.Api.Repositories;

public interface IFlashcardRepository
{
    Task<Flashcard?> GetByIdAsync(long id);
    Task<List<Flashcard>> GetAllAsync();
    Task AddAsync(Flashcard flashcard);
    Task<bool> SaveChangesAsync();
}
