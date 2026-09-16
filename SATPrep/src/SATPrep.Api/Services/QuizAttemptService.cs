using SATPrep.Api.DTOs;
using SATPrep.Api.Entities;
using SATPrep.Api.Mappings;
using SATPrep.Api.Repositories;

namespace SATPrep.Api.Services;

public class QuizAttemptService : IQuizAttemptService
{
    private readonly IQuizAttemptRepository _attemptRepository;
    private readonly IQuizRepository _quizRepository;
    private readonly IQuestionRepository _questionRepository;
    private readonly IUserProgressRepository _progressRepository;
    private readonly IStudySessionRepository _studySessionRepository;

    public QuizAttemptService(
        IQuizAttemptRepository attemptRepository,
        IQuizRepository quizRepository,
        IQuestionRepository questionRepository,
        IUserProgressRepository progressRepository,
        IStudySessionRepository studySessionRepository)
    {
        _attemptRepository = attemptRepository;
        _quizRepository = quizRepository;
        _questionRepository = questionRepository;
        _progressRepository = progressRepository;
        _studySessionRepository = studySessionRepository;
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
        var attempts = await _attemptRepository.GetByUserAsync(userId);
        return attempts.Select(a => a.ToGetDto()).ToList();
    }

    public async Task<List<QuizAttemptGetDto>> GetByQuizAsync(long quizId)
    {
        var attempts = await _attemptRepository.GetByQuizAsync(quizId);
        return attempts.Select(a => a.ToGetDto()).ToList();
    }

    public async Task<QuizAttemptGetDto?> StartAsync(long userId, long quizId)
    {
        var quiz = await _quizRepository.GetByIdAsync(quizId);
        if (quiz is null)
            return null;

        var totalQuestions = quiz.QuizQuestions.Count;

        var attempt = new QuizAttempt
        {
            QuizId = quizId,
            UserId = userId,
            StartedAt = DateTime.UtcNow,
            CompletedAt = null,
            Status = AttemptStatus.InProgress,
            Score = 0,
            PointsCorrect = 0,
            TotalQuestions = totalQuestions
        };

        await _attemptRepository.AddAsync(attempt);
        if (!await _attemptRepository.SaveChangesAsync())
            return null;

        return attempt.ToGetDto();
    }

    public async Task<QuizAttemptResultDto?> CompleteAsync(long attemptId, long userId, CompleteQuizAttemptDto dto)
    {
        var attempt = await _attemptRepository.GetByIdAsync(attemptId);
        if (attempt is null || attempt.UserId != userId)
            return null;

        if (attempt.Status != AttemptStatus.InProgress)
            return null;

        attempt.TimeTakenSeconds = dto.TimeTakenSeconds;
        attempt.CompletedAt = DateTime.UtcNow;

        // Grade each answer
        int correctCount = 0;
        var questionResults = new List<QuestionResultDto>();

        foreach (var answerDto in dto.Answers)
        {
            var question = await _questionRepository.GetByIdAsync(answerDto.QuestionId);
            if (question is null) continue;

            var correctChoice = question.Choices.FirstOrDefault(c => c.IsCorrect);
            var isCorrect = answerDto.SelectedChoiceId.HasValue &&
                            correctChoice != null &&
                            answerDto.SelectedChoiceId.Value == correctChoice.ChoiceId;

            if (isCorrect) correctCount++;

            attempt.Answers.Add(new Answer
            {
                QuizAttemptId = attemptId,
                QuestionId = answerDto.QuestionId,
                SelectedChoiceId = answerDto.SelectedChoiceId,
                IsCorrect = isCorrect,
                AnsweredAt = DateTime.UtcNow
            });
        }

        attempt.PointsCorrect = correctCount;
        attempt.Score = attempt.TotalQuestions > 0
            ? (int)Math.Round((double)correctCount / attempt.TotalQuestions * 100)
            : 0;
        attempt.Status = AttemptStatus.Completed;

        await _attemptRepository.SaveChangesAsync();

        // Update user progress by topic
        await UpdateUserProgressAsync(userId, dto.Answers);

        // Update study session
        var timeStudied = (dto.TimeTakenSeconds ?? 0) / 60;
        await UpdateStudySessionAsync(userId, Math.Max(timeStudied, 1));

        return await BuildResultDtoAsync(attempt);
    }

    public async Task<bool> DeleteAsync(long attemptId)
    {
        var attempt = await _attemptRepository.GetByIdAsync(attemptId);
        if (attempt is null)
            return false;

        _attemptRepository.Remove(attempt);
        return await _attemptRepository.SaveChangesAsync();
    }

    private async Task UpdateUserProgressAsync(long userId, List<AnswerCreateDto> answers)
    {
        foreach (var answerDto in answers)
        {
            var question = await _questionRepository.GetByIdAsync(answerDto.QuestionId);
            if (question is null) continue;

            var progress = await _progressRepository.GetByUserAndTopicAsync(userId, question.TopicId);
            if (progress is null)
            {
                progress = new UserProgress
                {
                    UserId = userId,
                    TopicId = question.TopicId,
                    QuestionsAttempted = 0,
                    CorrectAnswers = 0
                };
                await _progressRepository.AddAsync(progress);
            }

            progress.QuestionsAttempted++;
            var correctChoice = question.Choices.FirstOrDefault(c => c.IsCorrect);
            if (answerDto.SelectedChoiceId.HasValue && correctChoice != null &&
                answerDto.SelectedChoiceId.Value == correctChoice.ChoiceId)
            {
                progress.CorrectAnswers++;
            }
        }

        await _progressRepository.SaveChangesAsync();
    }

    private async Task UpdateStudySessionAsync(long userId, int minutesStudied)
    {
        var today = DateTime.UtcNow;
        var session = await _studySessionRepository.GetByUserAndDateAsync(userId, today);
        if (session is null)
        {
            session = new StudySession
            {
                UserId = userId,
                Date = today.Date,
                MinutesStudied = 0,
                QuestionsAnswered = 0,
                CorrectAnswers = 0
            };
            await _studySessionRepository.AddAsync(session);
        }

        session.MinutesStudied += minutesStudied;
        await _studySessionRepository.SaveChangesAsync();
    }

    private async Task<QuizAttemptResultDto> BuildResultDtoAsync(QuizAttempt attempt)
    {
        var quiz = await _quizRepository.GetByIdAsync(attempt.QuizId);
        var questionResults = new List<QuestionResultDto>();

        foreach (var answer in attempt.Answers)
        {
            var question = await _questionRepository.GetByIdAsync(answer.QuestionId);
            if (question is null) continue;

            var correctChoice = question.Choices.FirstOrDefault(c => c.IsCorrect);
            questionResults.Add(new QuestionResultDto(
                question.QuestionId,
                question.Text,
                question.Difficulty.ToString(),
                question.TopicId,
                question.Topic?.Name ?? "",
                question.Topic?.Subject?.Name ?? "",
                question.Choices.Select(c => new ChoiceGetDto(c.ChoiceId, c.Text)).ToArray(),
                answer.SelectedChoiceId,
                correctChoice?.ChoiceId,
                answer.IsCorrect));
        }

        var sectionBreakdown = questionResults
            .GroupBy(q => new { q.SubjectName, q.TopicName })
            .Select(g => new ScoreDetailDto(
                g.Key.TopicName,
                g.Count(x => x.IsCorrect),
                g.Count(),
                g.Count() > 0 ? (int)Math.Round((double)g.Count(x => x.IsCorrect) / g.Count() * 100) : 0));

        var sectionScore = new SectionScoreDto("Breakdown", sectionBreakdown);

        return new QuizAttemptResultDto(
            attempt.QuizAttemptId,
            attempt.QuizId,
            quiz?.Title ?? "",
            attempt.Score,
            attempt.PointsCorrect,
            attempt.TotalQuestions,
            attempt.TimeTakenSeconds,
            attempt.StartedAt,
            attempt.CompletedAt ?? DateTime.UtcNow,
            questionResults,
            new[] { sectionScore });
    }
}