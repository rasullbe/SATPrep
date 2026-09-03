using SATPrep.Api.Entities;

namespace SATPrep.Api.Data.DataSeeder;

public static class SubjectSeeder
{
    public static async Task Seed(AppDbContext context)
    {
        if (context.Subjects.Any()) return;

        var subjects = new List<Subject>
        {
            new() { Name = "Math" },
            new() { Name = "Reading" },
            new() { Name = "Writing" },
        };

        await context.Subjects.AddRangeAsync(subjects);
        await context.SaveChangesAsync();
    }
}
