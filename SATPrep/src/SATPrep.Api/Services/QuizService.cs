using SATPrep.Api.DTOs;
using SATPrep.Api.Mappings;
using SATPrep.Api.Repositories;

namespace SATPrep.Api.Services;

public class QuizService : IQuizService
{
    private readonly IQuizRepository _quizRepository;

    public QuizService(IQuizRepository quizRepository)
    {
        _quizRepository = quizRepository;
    }

    public async Task<QuizGetDto?> GetByIdAsync(long quizId)
    {
        var quiz = await _quizRepository.GetByIdAsync(quizId);
        return quiz?.ToGetDto();
    }

    public async Task<QuizDetailGetDto?> GetDetailByIdAsync(long quizId)
    {
        var quiz = await _quizRepository.GetByIdAsync(quizId);
        return quiz?.ToDetailGetDto();
    }

    public async Task<List<QuizGetDto>> GetAllAsync()
    {
        var quizzes = await _quizRepository.GetAllAsync();
        return quizzes.Select(q => q.ToGetDto()).ToList();
    }

    public async Task<List<QuizGetDto>> GetByCreatorAsync(long creatorId)
    {
        var quizzes = await _quizRepository.GetAllAsync();
        return quizzes.Where(q => q.CreatedById == creatorId).Select(q => q.ToGetDto()).ToList();
    }

    public async Task<QuizGetDto?> CreateAsync(QuizCreateDto createDto)
    {
        if (createDto is null)
            throw new ArgumentNullException(nameof(createDto));

        var quiz = createDto.ToEntity();
        await _quizRepository.AddAsync(quiz);

        if (!await _quizRepository.SaveChangesAsync())
            return null;

        return quiz.ToGetDto();
    }

    public async Task<QuizGetDto?> UpdateAsync(long quizId, QuizCreateDto updateDto)
    {
        if (updateDto is null)
            throw new ArgumentNullException(nameof(updateDto));

        var quiz = await _quizRepository.GetByIdAsync(quizId);
        if (quiz is null)
            return null;

        quiz.Title = updateDto.Title?.Trim() ?? string.Empty;
        quiz.Description = updateDto.Description ?? string.Empty;

        if (!await _quizRepository.SaveChangesAsync())
            return null;

        return quiz.ToGetDto();
    }

    public async Task<bool> DeleteAsync(long quizId)
    {
        var quiz = await _quizRepository.GetByIdAsync(quizId);
        if (quiz is null)
            return false;

        return await _quizRepository.SaveChangesAsync();
    }

    public async Task<bool> AddQuestionAsync(long quizId, long questionId, int order)
    {
        var quiz = await _quizRepository.GetByIdAsync(quizId);
        if (quiz is null)
            return false;

        quiz.QuizQuestions.Add(new() { QuizId = quizId, QuestionId = questionId, Order = order });
        return await _quizRepository.SaveChangesAsync();
    }

    public async Task<bool> RemoveQuestionAsync(long quizId, long questionId)
    {
        var quiz = await _quizRepository.GetByIdAsync(quizId);
        if (quiz is null)
            return false;

        var qq = quiz.QuizQuestions.FirstOrDefault(x => x.QuestionId == questionId);
        if (qq is null)
            return false;

        quiz.QuizQuestions.Remove(qq);
        return await _quizRepository.SaveChangesAsync();
    }
}
