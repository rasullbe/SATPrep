using SATPrep.Api.DTOs;
using SATPrep.Api.Mappings;
using SATPrep.Api.Repositories;

namespace SATPrep.Api.Services;

public class QuizAttemptService : IQuizAttemptService
{
    private readonly IQuizAttemptRepository _attemptRepository;

    public QuizAttemptService(IQuizAttemptRepository attemptRepository)
    {
        _attemptRepository = attemptRepository;
    }

    public async Task<QuizAttemptGetDto?> GetByIdAsync(long attemptId)
    {
        var attempt = await _attemptRepository.GetByIdAsync(attemptId);
        return attempt?.ToGetDto();
    }

    public async Task<List<QuizAttemptGetDto>> GetAllAsync()
    {
        var attempts = await _attemptRepository.GetAllAsync();
        return attempts.Select(a => a.ToGetDto()).ToList();
    }

    public async Task<List<QuizAttemptGetDto>> GetByUserAsync(long userId)
    {
        var attempts = await _attemptRepository.GetAllAsync();
        return attempts.Where(a => a.UserId == userId).Select(a => a.ToGetDto()).ToList();
    }

    public async Task<List<QuizAttemptGetDto>> GetByQuizAsync(long quizId)
    {
        var attempts = await _attemptRepository.GetAllAsync();
        return attempts.Where(a => a.QuizId == quizId).Select(a => a.ToGetDto()).ToList();
    }

    public async Task<QuizAttemptGetDto?> CreateAsync(QuizAttemptCreateDto createDto)
    {
        if (createDto is null)
            throw new ArgumentNullException(nameof(createDto));

        var attempt = createDto.ToEntity();
        await _attemptRepository.AddAsync(attempt);

        if (!await _attemptRepository.SaveChangesAsync())
            return null;

        return attempt.ToGetDto();
    }

    public async Task<bool> CompleteAsync(long attemptId, int score)
    {
        var attempt = await _attemptRepository.GetByIdAsync(attemptId);
        if (attempt is null)
            return false;

        attempt.CompletedAt = DateTime.UtcNow;
        attempt.Score = score;

        return await _attemptRepository.SaveChangesAsync();
    }

    public async Task<bool> DeleteAsync(long attemptId)
    {
        var attempt = await _attemptRepository.GetByIdAsync(attemptId);
        if (attempt is null)
            return false;

        return await _attemptRepository.SaveChangesAsync();
    }
}
