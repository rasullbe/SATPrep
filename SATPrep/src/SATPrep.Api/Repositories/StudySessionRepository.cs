using Microsoft.EntityFrameworkCore;
using SATPrep.Api.Data;
using SATPrep.Api.Entities;

namespace SATPrep.Api.Repositories;

public class StudySessionRepository : IStudySessionRepository
{
    private readonly AppDbContext _context;

    public StudySessionRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<StudySession?> GetByUserAndDateAsync(long userId, DateTime date)
    {
        var dateOnly = date.Date;
        return await _context.StudySessions
            .FirstOrDefaultAsync(s => s.UserId == userId && s.Date == dateOnly);
    }

    public async Task<List<StudySession>> GetByUserAsync(long userId)
    {
        return await _context.StudySessions
            .Where(s => s.UserId == userId)
            .OrderByDescending(s => s.Date)
            .ToListAsync();
    }

    public async Task<int> GetTotalSessionCountAsync(long userId)
    {
        return await _context.StudySessions.CountAsync(s => s.UserId == userId);
    }

    public async Task AddAsync(StudySession session)
    {
        await _context.StudySessions.AddAsync(session);
    }

    public async Task<bool> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync() > 0;
    }
}