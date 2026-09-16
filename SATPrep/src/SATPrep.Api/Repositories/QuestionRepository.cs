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
        return await _context.Questions
            .Include(q => q.Choices)
            .Include(q => q.QuestionTags).ThenInclude(qt => qt.Tag)
            .FirstOrDefaultAsync(q => q.QuestionId == id);
    }

    public async Task<List<Question>> GetAllAsync()
    {
        return await _context.Questions
            .Include(q => q.Choices)
            .Include(q => q.QuestionTags).ThenInclude(qt => qt.Tag)
            .ToListAsync();
    }

    public async Task<List<Question>> GetByTopicAsync(long topicId)
    {
        return await _context.Questions
            .Where(q => q.TopicId == topicId)
            .Include(q => q.Choices)
            .Include(q => q.QuestionTags).ThenInclude(qt => qt.Tag)
            .ToListAsync();
    }

    public async Task AddAsync(Question question)
    {
        await _context.Questions.AddAsync(question);
    }

    public void Remove(Question question)
    {
        _context.Questions.Remove(question);
    }

    public async Task<bool> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync() > 0;
    }
}