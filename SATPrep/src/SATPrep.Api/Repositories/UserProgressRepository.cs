using Microsoft.EntityFrameworkCore;
using SATPrep.Api.Data;
using SATPrep.Api.Entities;

namespace SATPrep.Api.Repositories;

public class UserProgressRepository : IUserProgressRepository
{
    private readonly AppDbContext _context;

    public UserProgressRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<UserProgress?> GetByUserAndTopicAsync(long userId, long topicId)
    {
        return await _context.UserProgresses
            .FirstOrDefaultAsync(p => p.UserId == userId && p.TopicId == topicId);
    }

    public async Task<List<UserProgress>> GetByUserAsync(long userId)
    {
        return await _context.UserProgresses
            .Where(p => p.UserId == userId)
            .ToListAsync();
    }

    public async Task AddAsync(UserProgress progress)
    {
        await _context.UserProgresses.AddAsync(progress);
    }

    public async Task<bool> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync() > 0;
    }
}