using SATPrep.Api.Entities;

namespace SATPrep.Api.Repositories;

public interface IUserProgressRepository
{
    Task<UserProgress?> GetByUserAndTopicAsync(long userId, long topicId);
    Task<List<UserProgress>> GetByUserAsync(long userId);
    Task AddAsync(UserProgress progress);
    Task<bool> SaveChangesAsync();
}