using SATPrep.Api.Entities;

namespace SATPrep.Api.Repositories;

public interface IQuizRepository
{
    Task<Quiz?> GetByIdAsync(long id);
    Task<Quiz?> GetByTitleAsync(string title);
    Task<List<Quiz>> GetAllAsync();
    Task<List<Quiz>> GetByCreatorAsync(long creatorId);
    Task<List<Quiz>> GetPublishedAsync();
    Task AddAsync(Quiz quiz);
    void Remove(Quiz quiz);
    Task<bool> SaveChangesAsync();
}