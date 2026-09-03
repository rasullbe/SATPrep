using SATPrep.Api.Entities;

namespace SATPrep.Api.Data.DataSeeder;

public static class TopicSeeder
{
    public static async Task Seed(AppDbContext context)
    {
        if (context.Topics.Any()) return;

        var readingWriting = context.Subjects.FirstOrDefault(s => s.Name == "Reading and Writing");
        var math = context.Subjects.FirstOrDefault(s => s.Name == "Math");

        var topics = new List<Topic>();

        // Reading and Writing domains (SAT 2026 format)
        if (readingWriting is not null)
        {
            topics.Add(new Topic { Name = "Craft and Structure", SubjectId = readingWriting.SubjectId });
            topics.Add(new Topic { Name = "Information and Ideas", SubjectId = readingWriting.SubjectId });
            topics.Add(new Topic { Name = "Standard English Conventions", SubjectId = readingWriting.SubjectId });
            topics.Add(new Topic { Name = "Expression of Ideas", SubjectId = readingWriting.SubjectId });
        }

        // Math domains (SAT 2026 format)
        if (math is not null)
        {
            topics.Add(new Topic { Name = "Algebra", SubjectId = math.SubjectId });
            topics.Add(new Topic { Name = "Advanced Math", SubjectId = math.SubjectId });
            topics.Add(new Topic { Name = "Geometry and Trigonometry", SubjectId = math.SubjectId });
            topics.Add(new Topic { Name = "Problem-Solving and Data Analysis", SubjectId = math.SubjectId });
        }

        if (topics.Any())
        {
            await context.Topics.AddRangeAsync(topics);
            await context.SaveChangesAsync();
        }
    }
}
