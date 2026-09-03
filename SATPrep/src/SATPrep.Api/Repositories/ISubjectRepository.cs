using SATPrep.Api.Entities;

namespace SATPrep.Api.Repositories;

public interface ISubjectRepository
{
    Task<Subject?> GetByIdAsync(long id);
    Task<Subject?> GetByNameAsync(string name);
    Task<List<Subject>> GetAllAsync();
    Task AddAsync(Subject subject);
    Task<bool> SaveChangesAsync();
}
