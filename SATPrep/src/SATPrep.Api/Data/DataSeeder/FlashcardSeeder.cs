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
            // Math vocabulary/formulas
            new() { UserId = user.UserId, Front = "Pythagorean Theorem", Back = "In a right triangle: a² + b² = c², where c is the hypotenuse" },
            new() { UserId = user.UserId, Front = "Slope formula", Back = "m = (y₂ - y₁) / (x₂ - x₁)" },
            new() { UserId = user.UserId, Front = "Quadratic formula", Back = "x = [-b ± √(b² - 4ac)] / 2a" },
            // Reading/Writing vocabulary and grammar
            new() { UserId = user.UserId, Front = "Paradox (rhetoric)", Back = "An apparent contradiction that may reveal a deeper truth; used in persuasive writing" },
            new() { UserId = user.UserId, Front = "Subject-verb agreement rule", Back = "Singular subjects take singular verbs; plural subjects take plural verbs (e.g., 'The team is...' vs 'The players are...')" },
            new() { UserId = user.UserId, Front = "Parallel structure", Back = "Using the same grammatical form for words in a series (e.g., 'running, jumping, and swimming' not 'running, jumping, and swim')" },
        };

        await context.Flashcards.AddRangeAsync(flashcards);
        await context.SaveChangesAsync();
    }
}
