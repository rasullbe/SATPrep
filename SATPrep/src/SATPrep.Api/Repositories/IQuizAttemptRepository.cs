using SATPrep.Api.Entities;

namespace SATPrep.Api.Repositories;

public interface IQuizAttemptRepository
{
    Task<QuizAttempt?> GetByIdAsync(long id);
    Task<List<QuizAttempt>> GetAllAsync();
    Task AddAsync(QuizAttempt attempt);
    Task<bool> SaveChangesAsync();
}
