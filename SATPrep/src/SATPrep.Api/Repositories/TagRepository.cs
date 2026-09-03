using Microsoft.EntityFrameworkCore;
using SATPrep.Api.Data;
using SATPrep.Api.Entities;

namespace SATPrep.Api.Repositories;

public class TagRepository : ITagRepository
{
    private readonly AppDbContext _context;

    public TagRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Tag?> GetByIdAsync(long id)
    {
        return await _context.Tags.FindAsync(id);
    }

    public async Task<Tag?> GetByNameAsync(string name)
    {
        return await _context.Tags
            .FirstOrDefaultAsync(t => t.Name == name);
    }

    public async Task<List<Tag>> GetAllAsync()
    {
        return await _context.Tags.ToListAsync();
    }

    public async Task AddAsync(Tag tag)
    {
        await _context.Tags.AddAsync(tag);
    }

    public async Task<bool> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync() > 0;
    }
}
