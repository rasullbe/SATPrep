using SATPrep.Api.DTOs;

namespace SATPrep.Api.Services;

public interface ISubjectService
{
    Task<SubjectGetDto?> GetByIdAsync(long subjectId);
    Task<SubjectGetDto?> GetByNameAsync(string name);
    Task<List<SubjectGetDto>> GetAllAsync();
    Task<SubjectGetDto?> CreateAsync(SubjectCreateDto createDto);
    Task<SubjectGetDto?> UpdateAsync(long subjectId, SubjectCreateDto updateDto);
    Task<bool> DeleteAsync(long subjectId);
}
