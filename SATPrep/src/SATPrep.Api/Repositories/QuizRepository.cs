using Microsoft.EntityFrameworkCore;
using SATPrep.Api.Data;
using SATPrep.Api.Entities;

namespace SATPrep.Api.Repositories;

public class QuizRepository : IQuizRepository
{
    private readonly AppDbContext _context;

    public QuizRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Quiz?> GetByIdAsync(long id)
    {
        return await _context.Quizzes
            .Include(q => q.QuizQuestions).ThenInclude(qq => qq.Question)
            .FirstOrDefaultAsync(q => q.QuizId == id);
    }

    public async Task<Quiz?> GetByTitleAsync(string title)
    {
        return await _context.Quizzes
            .FirstOrDefaultAsync(q => q.Title == title);
    }

    public async Task<List<Quiz>> GetAllAsync()
    {
        return await _context.Quizzes
            .Include(q => q.QuizQuestions)
            .ToListAsync();
    }

    public async Task<List<Quiz>> GetByCreatorAsync(long creatorId)
    {
        return await _context.Quizzes
            .Where(q => q.CreatedById == creatorId)
            .Include(q => q.QuizQuestions)
            .ToListAsync();
    }

    public async Task<List<Quiz>> GetPublishedAsync()
    {
        return await _context.Quizzes
            .Where(q => q.IsPublished)
            .Include(q => q.QuizQuestions)
            .ToListAsync();
    }

    public async Task AddAsync(Quiz quiz)
    {
        await _context.Quizzes.AddAsync(quiz);
    }

    public void Remove(Quiz quiz)
    {
        _context.Quizzes.Remove(quiz);
    }

    public async Task<bool> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync() > 0;
    }
}