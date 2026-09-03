using SATPrep.Api.Entities;

namespace SATPrep.Api.Data.DataSeeder;

public static class TagSeeder
{
    public static async Task Seed(AppDbContext context)
    {
        if (context.Tags.Any()) return;

        var tags = new List<Tag>
        {
            new() { Name = "algebra" },
            new() { Name = "geometry" },
            new() { Name = "vocabulary" },
            new() { Name = "grammar" },
            new() { Name = "passage" },
        };

        await context.Tags.AddRangeAsync(tags);
        await context.SaveChangesAsync();
    }
}
