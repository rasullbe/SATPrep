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
        return await _context.QuizAttempts.FindAsync(id);
    }

    public async Task<List<QuizAttempt>> GetAllAsync()
    {
        return await _context.QuizAttempts.ToListAsync();
    }

    public async Task AddAsync(QuizAttempt attempt)
    {
        await _context.QuizAttempts.AddAsync(attempt);
    }

    public async Task<bool> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync() > 0;
    }
}
