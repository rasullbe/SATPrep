using SATPrep.Api.DTOs;
using SATPrep.Api.Mappings;
using SATPrep.Api.Repositories;

namespace SATPrep.Api.Services;

public class QuestionService : IQuestionService
{
    private readonly IQuestionRepository _questionRepository;
    private readonly ITagRepository _tagRepository;

    public QuestionService(IQuestionRepository questionRepository, ITagRepository tagRepository)
    {
        _questionRepository = questionRepository;
        _tagRepository = tagRepository;
    }

    public async Task<QuestionGetDto?> GetByIdAsync(long questionId)
    {
        var question = await _questionRepository.GetByIdAsync(questionId);
        return question?.ToGetDto();
    }

    public async Task<QuestionAdminDto?> GetByIdAdminAsync(long questionId)
    {
        var question = await _questionRepository.GetByIdAsync(questionId);
        return question?.ToAdminDto();
    }

    public async Task<List<QuestionGetDto>> GetAllAsync()
    {
        var questions = await _questionRepository.GetAllAsync();
        return questions.Select(q => q.ToGetDto()).ToList();
    }

    public async Task<List<QuestionGetDto>> GetByTopicAsync(long topicId)
    {
        var questions = await _questionRepository.GetAllAsync();
        return questions.Where(q => q.TopicId == topicId).Select(q => q.ToGetDto()).ToList();
    }

    public async Task<QuestionGetDto?> CreateAsync(QuestionCreateDto createDto)
    {
        if (createDto is null)
            throw new ArgumentNullException(nameof(createDto));

        var question = createDto.ToEntity();
        await _questionRepository.AddAsync(question);

        if (!await _questionRepository.SaveChangesAsync())
            return null;

        return question.ToGetDto();
    }

    public async Task<QuestionGetDto?> UpdateAsync(long questionId, QuestionUpdateDto updateDto)
    {
        if (updateDto is null)
            throw new ArgumentNullException(nameof(updateDto));

        var question = await _questionRepository.GetByIdAsync(questionId);
        if (question is null)
            return null;

        question.UpdateFrom(updateDto);

        if (!await _questionRepository.SaveChangesAsync())
            return null;

        return question.ToGetDto();
    }

    public async Task<bool> DeleteAsync(long questionId)
    {
        var question = await _questionRepository.GetByIdAsync(questionId);
        if (question is null)
            return false;

        return await _questionRepository.SaveChangesAsync();
    }

    public async Task<bool> AddTagAsync(long questionId, long tagId)
    {
        var question = await _questionRepository.GetByIdAsync(questionId);
        var tag = await _tagRepository.GetByIdAsync(tagId);

        if (question is null || tag is null)
            return false;

        var hasTag = question.QuestionTags.Any(qt => qt.TagId == tagId);
        if (hasTag)
            return true;

        question.QuestionTags.Add(new() { QuestionId = questionId, TagId = tagId });
        return await _questionRepository.SaveChangesAsync();
    }

    public async Task<bool> RemoveTagAsync(long questionId, long tagId)
    {
        var question = await _questionRepository.GetByIdAsync(questionId);
        if (question is null)
            return false;

        var qt = question.QuestionTags.FirstOrDefault(x => x.TagId == tagId);
        if (qt is null)
            return false;

        question.QuestionTags.Remove(qt);
        return await _questionRepository.SaveChangesAsync();
    }
}
