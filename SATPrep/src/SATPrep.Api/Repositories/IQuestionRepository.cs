using SATPrep.Api.Entities;

namespace SATPrep.Api.Repositories;

public interface IQuestionRepository
{
    Task<Question?> GetByIdAsync(long id);
    Task<List<Question>> GetAllAsync();
    Task AddAsync(Question question);
    Task<bool> SaveChangesAsync();
}
