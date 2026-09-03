using SATPrep.Api.Entities;

namespace SATPrep.Api.Data.DataSeeder;

public static class FlashcardSeeder
{
    public static async Task Seed(AppDbContext context)
    {
        if (context.Flashcards.Any()) return;

        var user = context.Users.FirstOrDefault();
        if (user is null) return;

        var flashcards = new List<Flashcard>
        {
            new() { UserId = user.UserId, Front = "Pythagorean theorem", Back = "a^2 + b^2 = c^2" },
            new() { UserId = user.UserId, Front = "Verb tense: past of 'go'", Back = "went" },
        };

        await context.Flashcards.AddRangeAsync(flashcards);
        await context.SaveChangesAsync();
    }
}
