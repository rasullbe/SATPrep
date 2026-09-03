using SATPrep.Api.DTOs;

namespace SATPrep.Api.Services;

public interface ITopicService
{
    Task<TopicGetDto?> GetByIdAsync(long topicId);
    Task<TopicGetDto?> GetByNameAsync(string name);
    Task<List<TopicGetDto>> GetAllAsync();
    Task<List<TopicGetDto>> GetBySubjectAsync(long subjectId);
    Task<TopicGetDto?> CreateAsync(TopicCreateDto createDto);
    Task<TopicGetDto?> UpdateAsync(long topicId, TopicCreateDto updateDto);
    Task<bool> DeleteAsync(long topicId);
}
