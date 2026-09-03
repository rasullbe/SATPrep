using SATPrep.Api.DTOs;
using SATPrep.Api.Mappings;
using SATPrep.Api.Repositories;

namespace SATPrep.Api.Services;

public class TagService : ITagService
{
    private readonly ITagRepository _tagRepository;

    public TagService(ITagRepository tagRepository)
    {
        _tagRepository = tagRepository;
    }

    public async Task<TagGetDto?> GetByIdAsync(long tagId)
    {
        var tag = await _tagRepository.GetByIdAsync(tagId);
        return tag?.ToGetDto();
    }

    public async Task<TagGetDto?> GetByNameAsync(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return null;

        var tag = await _tagRepository.GetByNameAsync(name);
        return tag?.ToGetDto();
    }

    public async Task<List<TagGetDto>> GetAllAsync()
    {
        var tags = await _tagRepository.GetAllAsync();
        return tags.Select(t => t.ToGetDto()).ToList();
    }

    public async Task<TagGetDto?> CreateAsync(TagCreateDto createDto)
    {
        if (createDto is null)
            throw new ArgumentNullException(nameof(createDto));

        var tag = createDto.ToEntity();
        await _tagRepository.AddAsync(tag);

        if (!await _tagRepository.SaveChangesAsync())
            return null;

        return tag.ToGetDto();
    }

    public async Task<TagGetDto?> UpdateAsync(long tagId, TagCreateDto updateDto)
    {
        if (updateDto is null)
            throw new ArgumentNullException(nameof(updateDto));

        var tag = await _tagRepository.GetByIdAsync(tagId);
        if (tag is null)
            return null;

        tag.Name = updateDto.Name?.Trim() ?? string.Empty;

        if (!await _tagRepository.SaveChangesAsync())
            return null;

        return tag.ToGetDto();
    }

    public async Task<bool> DeleteAsync(long tagId)
    {
        var tag = await _tagRepository.GetByIdAsync(tagId);
        if (tag is null)
            return false;

        return await _tagRepository.SaveChangesAsync();
    }
}
