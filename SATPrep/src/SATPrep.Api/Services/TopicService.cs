using SATPrep.Api.DTOs;
using SATPrep.Api.Mappings;
using SATPrep.Api.Repositories;

namespace SATPrep.Api.Services;

public class TopicService : ITopicService
{
    private readonly ITopicRepository _topicRepository;

    public TopicService(ITopicRepository topicRepository)
    {
        _topicRepository = topicRepository;
    }

    public async Task<TopicGetDto?> GetByIdAsync(long topicId)
    {
        var topic = await _topicRepository.GetByIdAsync(topicId);
        return topic?.ToGetDto();
    }

    public async Task<TopicGetDto?> GetByNameAsync(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return null;

        var topic = await _topicRepository.GetByNameAsync(name);
        return topic?.ToGetDto();
    }

    public async Task<List<TopicGetDto>> GetAllAsync()
    {
        var topics = await _topicRepository.GetAllAsync();
        return topics.Select(t => t.ToGetDto()).ToList();
    }

    public async Task<List<TopicGetDto>> GetBySubjectAsync(long subjectId)
    {
        var topics = await _topicRepository.GetAllAsync();
        return topics.Where(t => t.SubjectId == subjectId).Select(t => t.ToGetDto()).ToList();
    }

    public async Task<TopicGetDto?> CreateAsync(TopicCreateDto createDto)
    {
        if (createDto is null)
            throw new ArgumentNullException(nameof(createDto));

        var topic = createDto.ToEntity();
        await _topicRepository.AddAsync(topic);

        if (!await _topicRepository.SaveChangesAsync())
            return null;

        return topic.ToGetDto();
    }

    public async Task<TopicGetDto?> UpdateAsync(long topicId, TopicCreateDto updateDto)
    {
        if (updateDto is null)
            throw new ArgumentNullException(nameof(updateDto));

        var topic = await _topicRepository.GetByIdAsync(topicId);
        if (topic is null)
            return null;

        topic.Name = updateDto.Name?.Trim() ?? string.Empty;

        if (!await _topicRepository.SaveChangesAsync())
            return null;

        return topic.ToGetDto();
    }

    public async Task<bool> DeleteAsync(long topicId)
    {
        var topic = await _topicRepository.GetByIdAsync(topicId);
        if (topic is null)
            return false;

        return await _topicRepository.SaveChangesAsync();
    }
}
