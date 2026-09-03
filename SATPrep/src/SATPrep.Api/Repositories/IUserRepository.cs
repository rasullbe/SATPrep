using SATPrep.Api.Entities;

namespace SATPrep.Api.Repositories;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(long id);
    Task<User?> GetByEmailAsync(string email);
    Task<List<User>> GetAllAsync();
    Task AddAsync(User user);
    Task<bool> SaveChangesAsync();
}