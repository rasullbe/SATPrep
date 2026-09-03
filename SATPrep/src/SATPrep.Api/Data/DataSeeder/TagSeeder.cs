using SATPrep.Api.Entities;

namespace SATPrep.Api.Data.DataSeeder;

public static class TagSeeder
{
    public static async Task Seed(AppDbContext context)
    {
        if (context.Tags.Any()) return;

        var tags = new List<Tag>
        {
            // Reading and Writing domains
            new() { Name = "craft-and-structure" },
            new() { Name = "information-and-ideas" },
            new() { Name = "standard-english-conventions" },
            new() { Name = "expression-of-ideas" },
            // Math domains
            new() { Name = "algebra" },
            new() { Name = "advanced-math" },
            new() { Name = "geometry-and-trigonometry" },
            new() { Name = "problem-solving-data-analysis" },
        };

        await context.Tags.AddRangeAsync(tags);
        await context.SaveChangesAsync();
    }
}
