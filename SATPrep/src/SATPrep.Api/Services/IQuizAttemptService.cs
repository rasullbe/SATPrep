using SATPrep.Api.DTOs;

namespace SATPrep.Api.Services;

public interface IQuizAttemptService
{
    Task<QuizAttemptGetDto?> GetByIdAsync(long attemptId);
    Task<List<QuizAttemptGetDto>> GetAllAsync();
    Task<List<QuizAttemptGetDto>> GetByUserAsync(long userId);
    Task<List<QuizAttemptGetDto>> GetByQuizAsync(long quizId);
    Task<QuizAttemptGetDto?> StartAsync(long userId, long quizId);
    Task<QuizAttemptResultDto?> CompleteAsync(long attemptId, long userId, CompleteQuizAttemptDto dto);
    Task<bool> DeleteAsync(long attemptId);
}