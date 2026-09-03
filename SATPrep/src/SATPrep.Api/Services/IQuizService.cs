using SATPrep.Api.DTOs;

namespace SATPrep.Api.Services;

public interface IQuizService
{
    Task<QuizGetDto?> GetByIdAsync(long quizId);
    Task<QuizDetailGetDto?> GetDetailByIdAsync(long quizId);
    Task<List<QuizGetDto>> GetAllAsync();
    Task<List<QuizGetDto>> GetByCreatorAsync(long creatorId);
    Task<QuizGetDto?> CreateAsync(QuizCreateDto createDto);
    Task<QuizGetDto?> UpdateAsync(long quizId, QuizCreateDto updateDto);
    Task<bool> DeleteAsync(long quizId);
    Task<bool> AddQuestionAsync(long quizId, long questionId, int order);
    Task<bool> RemoveQuestionAsync(long quizId, long questionId);
}
