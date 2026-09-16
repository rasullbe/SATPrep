using Microsoft.EntityFrameworkCore;
using SATPrep.Api.Data;
using SATPrep.Api.Entities;

namespace SATPrep.Api.Repositories;

public class QuizAttemptRepository : IQuizAttemptRepository
{
    private readonly AppDbContext _context;

    public QuizAttemptRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<QuizAttempt?> GetByIdAsync(long id)
    {
        return await _context.QuizAttempts
            .Include(a => a.Answers)
            .FirstOrDefaultAsync(a => a.QuizAttemptId == id);
    }

    public async Task<List<QuizAttempt>> GetAllAsync()
    {
        return await _context.QuizAttempts
            .Include(a => a.Answers)
            .ToListAsync();
    }

    public async Task<List<QuizAttempt>> GetByUserAsync(long userId)
    {
        return await _context.QuizAttempts
            .Where(a => a.UserId == userId)
            .Include(a => a.Answers)
            .OrderByDescending(a => a.StartedAt)
            .ToListAsync();
    }

    public async Task<List<QuizAttempt>> GetByQuizAsync(long quizId)
    {
        return await _context.QuizAttempts
            .Where(a => a.QuizId == quizId)
            .Include(a => a.Answers)
            .ToListAsync();
    }

    public async Task<int> GetUserTotalCorrectAsync(long userId)
    {
        return await _context.QuizAttempts
            .Where(a => a.UserId == userId && a.Status == AttemptStatus.Completed)
            .SumAsync(a => a.PointsCorrect);
    }

    public async Task AddAsync(QuizAttempt attempt)
    {
        await _context.QuizAttempts.AddAsync(attempt);
    }

    public void Remove(QuizAttempt attempt)
    {
        _context.QuizAttempts.Remove(attempt);
    }

    public async Task<bool> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync() > 0;
    }
}