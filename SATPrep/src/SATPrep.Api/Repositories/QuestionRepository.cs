using Microsoft.EntityFrameworkCore;
using SATPrep.Api.Data;
using SATPrep.Api.Entities;

namespace SATPrep.Api.Repositories;

public class QuestionRepository : IQuestionRepository
{
    private readonly AppDbContext _context;

    public QuestionRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Question?> GetByIdAsync(long id)
    {
        return await _context.Questions.FindAsync(id);
    }

    public async Task<List<Question>> GetAllAsync()
    {
        return await _context.Questions.ToListAsync();
    }

    public async Task AddAsync(Question question)
    {
        await _context.Questions.AddAsync(question);
    }

    public async Task<bool> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync() > 0;
    }
}
