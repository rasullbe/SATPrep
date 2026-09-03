using SATPrep.Api.Entities;

namespace SATPrep.Api.Repositories;

public interface ITagRepository
{
    Task<Tag?> GetByIdAsync(long id);
    Task<Tag?> GetByNameAsync(string name);
    Task<List<Tag>> GetAllAsync();
    Task AddAsync(Tag tag);
    Task<bool> SaveChangesAsync();
}
