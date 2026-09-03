using SATPrep.Api.DTOs;

namespace SATPrep.Api.Services;

public interface IQuizAttemptService
{
    Task<QuizAttemptGetDto?> GetByIdAsync(long attemptId);
    Task<List<QuizAttemptGetDto>> GetAllAsync();
    Task<List<QuizAttemptGetDto>> GetByUserAsync(long userId);
    Task<List<QuizAttemptGetDto>> GetByQuizAsync(long quizId);
    Task<QuizAttemptGetDto?> CreateAsync(QuizAttemptCreateDto createDto);
    Task<bool> CompleteAsync(long attemptId, int score);
    Task<bool> DeleteAsync(long attemptId);
}
