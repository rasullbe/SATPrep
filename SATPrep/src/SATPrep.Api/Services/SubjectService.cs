using SATPrep.Api.DTOs;
using SATPrep.Api.Mappings;
using SATPrep.Api.Repositories;

namespace SATPrep.Api.Services;

public class SubjectService : ISubjectService
{
    private readonly ISubjectRepository _subjectRepository;

    public SubjectService(ISubjectRepository subjectRepository)
    {
        _subjectRepository = subjectRepository;
    }

    public async Task<SubjectGetDto?> GetByIdAsync(long subjectId)
    {
        var subject = await _subjectRepository.GetByIdAsync(subjectId);
        return subject?.ToGetDto();
    }

    public async Task<SubjectGetDto?> GetByNameAsync(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return null;

        var subject = await _subjectRepository.GetByNameAsync(name);
        return subject?.ToGetDto();
    }

    public async Task<List<SubjectGetDto>> GetAllAsync()
    {
        var subjects = await _subjectRepository.GetAllAsync();
        return subjects.Select(s => s.ToGetDto()).ToList();
    }

    public async Task<SubjectGetDto?> CreateAsync(SubjectCreateDto createDto)
    {
        if (createDto is null)
            throw new ArgumentNullException(nameof(createDto));

        var subject = createDto.ToEntity();
        await _subjectRepository.AddAsync(subject);

        if (!await _subjectRepository.SaveChangesAsync())
            return null;

        return subject.ToGetDto();
    }

    public async Task<SubjectGetDto?> UpdateAsync(long subjectId, SubjectCreateDto updateDto)
    {
        if (updateDto is null)
            throw new ArgumentNullException(nameof(updateDto));

        var subject = await _subjectRepository.GetByIdAsync(subjectId);
        if (subject is null)
            return null;

        subject.Name = updateDto.Name?.Trim() ?? string.Empty;

        if (!await _subjectRepository.SaveChangesAsync())
            return null;

        return subject.ToGetDto();
    }

    public async Task<bool> DeleteAsync(long subjectId)
    {
        var subject = await _subjectRepository.GetByIdAsync(subjectId);
        if (subject is null)
            return false;

        return await _subjectRepository.SaveChangesAsync();
    }
}
