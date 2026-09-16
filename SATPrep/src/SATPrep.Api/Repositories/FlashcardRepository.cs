using Microsoft.EntityFrameworkCore;
using SATPrep.Api.Data;
using SATPrep.Api.Entities;

namespace SATPrep.Api.Repositories;

public class FlashcardRepository : IFlashcardRepository
{
    private readonly AppDbContext _context;

    public FlashcardRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Flashcard?> GetByIdAsync(long id)
    {
        return await _context.Flashcards.FindAsync(id);
    }

    public async Task<List<Flashcard>> GetAllAsync()
    {
        return await _context.Flashcards.ToListAsync();
    }

    public async Task<List<Flashcard>> GetByUserAsync(long userId)
    {
        return await _context.Flashcards
            .Where(f => f.UserId == userId)
            .ToListAsync();
    }

    public async Task AddAsync(Flashcard flashcard)
    {
        await _context.Flashcards.AddAsync(flashcard);
    }

    public void Remove(Flashcard flashcard)
    {
        _context.Flashcards.Remove(flashcard);
    }

    public async Task<bool> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync() > 0;
    }
}