using SATPrep.Api.Entities;

namespace SATPrep.Api.Repositories;

public interface ITopicRepository
{
    Task<Topic?> GetByIdAsync(long id);
    Task<Topic?> GetByNameAsync(string name);
    Task<List<Topic>> GetAllAsync();
    Task AddAsync(Topic topic);
    Task<bool> SaveChangesAsync();
}
