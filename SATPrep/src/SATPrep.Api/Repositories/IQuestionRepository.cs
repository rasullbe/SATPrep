using SATPrep.Api.Entities;

namespace SATPrep.Api.Repositories;

public interface IQuestionRepository
{
    Task<Question?> GetByIdAsync(long id);
    Task<List<Question>> GetAllAsync();
    Task<List<Question>> GetByTopicAsync(long topicId);
    Task AddAsync(Question question);
    void Remove(Question question);
    Task<bool> SaveChangesAsync();
}