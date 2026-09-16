using SATPrep.Api.Entities;

namespace SATPrep.Api.Repositories;

public interface IQuizAttemptRepository
{
    Task<QuizAttempt?> GetByIdAsync(long id);
    Task<List<QuizAttempt>> GetAllAsync();
    Task<List<QuizAttempt>> GetByUserAsync(long userId);
    Task<List<QuizAttempt>> GetByQuizAsync(long quizId);
    Task<int> GetUserTotalCorrectAsync(long userId);
    Task AddAsync(QuizAttempt attempt);
    void Remove(QuizAttempt attempt);
    Task<bool> SaveChangesAsync();
}