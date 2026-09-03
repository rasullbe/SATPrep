using SATPrep.Api.DTOs;

namespace SATPrep.Api.Services;

public interface ITagService
{
    Task<TagGetDto?> GetByIdAsync(long tagId);
    Task<TagGetDto?> GetByNameAsync(string name);
    Task<List<TagGetDto>> GetAllAsync();
    Task<TagGetDto?> CreateAsync(TagCreateDto createDto);
    Task<TagGetDto?> UpdateAsync(long tagId, TagCreateDto updateDto);
    Task<bool> DeleteAsync(long tagId);
}
