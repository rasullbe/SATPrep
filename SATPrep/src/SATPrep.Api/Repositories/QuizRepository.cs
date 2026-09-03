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
        return await _context.Quizzes.FindAsync(id);
    }

    public async Task<Quiz?> GetByTitleAsync(string title)
    {
        return await _context.Quizzes
            .FirstOrDefaultAsync(q => q.Title == title);
    }

    public async Task<List<Quiz>> GetAllAsync()
    {
        return await _context.Quizzes.ToListAsync();
    }

    public async Task AddAsync(Quiz quiz)
    {
        await _context.Quizzes.AddAsync(quiz);
    }

    public async Task<bool> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync() > 0;
    }
}
