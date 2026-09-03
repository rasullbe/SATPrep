using SATPrep.Api.DTOs;

namespace SATPrep.Api.Services;

public interface IQuestionService
{
    Task<QuestionGetDto?> GetByIdAsync(long questionId);
    Task<QuestionAdminDto?> GetByIdAdminAsync(long questionId);
    Task<List<QuestionGetDto>> GetAllAsync();
    Task<List<QuestionGetDto>> GetByTopicAsync(long topicId);
    Task<QuestionGetDto?> CreateAsync(QuestionCreateDto createDto);
    Task<QuestionGetDto?> UpdateAsync(long questionId, QuestionUpdateDto updateDto);
    Task<bool> DeleteAsync(long questionId);
    Task<bool> AddTagAsync(long questionId, long tagId);
    Task<bool> RemoveTagAsync(long questionId, long tagId);
}
