using Microsoft.EntityFrameworkCore;
using SATPrep.Api.Data;
using SATPrep.Api.Entities;

namespace SATPrep.Api.Repositories;

public class TopicRepository : ITopicRepository
{
    private readonly AppDbContext _context;

    public TopicRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Topic?> GetByIdAsync(long id)
    {
        return await _context.Topics.FindAsync(id);
    }

    public async Task<Topic?> GetByNameAsync(string name)
    {
        return await _context.Topics
            .FirstOrDefaultAsync(t => t.Name == name);
    }

    public async Task<List<Topic>> GetAllAsync()
    {
        return await _context.Topics.ToListAsync();
    }

    public async Task AddAsync(Topic topic)
    {
        await _context.Topics.AddAsync(topic);
    }

    public async Task<bool> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync() > 0;
    }
}
