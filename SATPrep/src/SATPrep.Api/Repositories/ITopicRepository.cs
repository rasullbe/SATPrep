using SATPrep.Api.Entities;

namespace SATPrep.Api.Repositories;

public interface ITopicRepository
{
    Task<Topic?> GetByIdAsync(long id);
    Task<Topic?> GetByNameAsync(string name);
    Task<List<Topic>> GetAllAsync();
    Task<List<Topic>> GetBySubjectAsync(long subjectId);
    Task AddAsync(Topic topic);
    void Remove(Topic topic);
    Task<bool> SaveChangesAsync();
}