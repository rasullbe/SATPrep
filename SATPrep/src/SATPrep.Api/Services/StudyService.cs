using SATPrep.Api.DTOs;
using SATPrep.Api.Entities;
using SATPrep.Api.Mappings;
using SATPrep.Api.Repositories;

namespace SATPrep.Api.Services;

public class StudyService : IStudyService
{
    private readonly IUserRepository _userRepository;
    private readonly IQuizAttemptRepository _attemptRepository;
    private readonly IQuizRepository _quizRepository;
    private readonly IStudySessionRepository _studySessionRepository;
    private readonly IUserProgressRepository _progressRepository;

    public StudyService(
        IUserRepository userRepository,
        IQuizAttemptRepository attemptRepository,
        IQuizRepository quizRepository,
        IStudySessionRepository studySessionRepository,
        IUserProgressRepository progressRepository)
    {
        _userRepository = userRepository;
        _attemptRepository = attemptRepository;
        _quizRepository = quizRepository;
        _studySessionRepository = studySessionRepository;
        _progressRepository = progressRepository;
    }

    public async Task<DashboardDto> GetDashboardAsync(long userId)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user is null)
            throw new SATPrep.Api.Exceptions.NotFoundException("User not found.");

        var totalSessionCount = await _studySessionRepository.GetTotalSessionCountAsync(userId);
        var totalCorrect = await _attemptRepository.GetUserTotalCorrectAsync(userId);

        var recentAttempts = await _attemptRepository.GetByUserAsync(userId);
        var recentDto = recentAttempts.Take(10).Select(a => a.ToGetDto());

        var quizzes = await _quizRepository.GetPublishedAsync();
        var availableQuizzes = quizzes.Where(q => q.Attempts.All(a => a.UserId != userId)).Take(5).Select(q => q.ToGetDto());
        if (!availableQuizzes.Any())
            availableQuizzes = quizzes.Take(5).Select(q => q.ToGetDto());

        var progressList = await _progressRepository.GetByUserAsync(userId);

        var weakAreas = progressList
            .Where(p => p.QuestionsAttempted >= 2)
            .Select(p => new TopicAccuracyDto(
                p.TopicId,
                p.Topic?.Name ?? "",
                p.Topic?.SubjectId ?? 0,
                p.Topic?.Subject?.Name ?? "",
                p.QuestionsAttempted,
                p.CorrectAnswers,
                p.QuestionsAttempted > 0
                    ? (int)Math.Round((double)p.CorrectAnswers / p.QuestionsAttempted * 100)
                    : 0))
            .OrderBy(p => p.AccuracyPercentage)
            .Take(5);

        var avgScore = recentAttempts.Count > 0
            ? (int)Math.Round(recentAttempts.Where(a => a.Status == AttemptStatus.Completed).DefaultIfEmpty().Average(a => a?.Score ?? 0))
            : 0;

        var userStats = new UserStatsDto(
            userId,
            user.Name,
            user.StudyStreak,
            user.TotalStudyMinutes,
            totalSessionCount,
            totalCorrect,
            user.LastStudiedAt,
            avgScore);

        return new DashboardDto(
            userStats,
            availableQuizzes.ToList(),
            recentDto.ToList(),
            weakAreas.ToList());
    }
}