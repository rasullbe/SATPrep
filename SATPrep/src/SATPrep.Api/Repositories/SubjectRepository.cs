using Microsoft.EntityFrameworkCore;
using SATPrep.Api.Data;
using SATPrep.Api.Entities;

namespace SATPrep.Api.Repositories;

public class SubjectRepository : ISubjectRepository
{
    private readonly AppDbContext _context;

    public SubjectRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Subject?> GetByIdAsync(long id)
    {
        return await _context.Subjects
            .Include(s => s.Topics)
            .FirstOrDefaultAsync(s => s.SubjectId == id);
    }

    public async Task<Subject?> GetByNameAsync(string name)
    {
        return await _context.Subjects
            .FirstOrDefaultAsync(s => s.Name == name);
    }

    public async Task<List<Subject>> GetAllAsync()
    {
        return await _context.Subjects
            .Include(s => s.Topics)
            .ToListAsync();
    }

    public async Task AddAsync(Subject subject)
    {
        await _context.Subjects.AddAsync(subject);
    }

    public void Remove(Subject subject)
    {
        _context.Subjects.Remove(subject);
    }

    public async Task<bool> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync() > 0;
    }
}