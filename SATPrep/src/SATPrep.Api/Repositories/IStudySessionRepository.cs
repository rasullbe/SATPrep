using SATPrep.Api.Entities;

namespace SATPrep.Api.Repositories;

public interface IStudySessionRepository
{
    Task<StudySession?> GetByUserAndDateAsync(long userId, DateTime date);
    Task<List<StudySession>> GetByUserAsync(long userId);
    Task<int> GetTotalSessionCountAsync(long userId);
    Task AddAsync(StudySession session);
    Task<bool> SaveChangesAsync();
}