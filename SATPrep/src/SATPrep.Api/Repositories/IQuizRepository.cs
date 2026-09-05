using SATPrep.Api.Entities;

namespace SATPrep.Api.Repositories;

public interface IQuizRepository
{
    Task<Quiz?> GetByIdAsync(long id);
    Task<Quiz?> GetByTitleAsync(string title);
    Task<List<Quiz>> GetAllAsync();
    Task AddAsync(Quiz quiz);
    Task<bool> SaveChangesAsync();
    void Remove(Quiz quiz);
}
