using SATPrep.Api.Entities;

namespace SATPrep.Api.Data.DataSeeder;

public static class TopicSeeder
{
    public static async Task Seed(AppDbContext context)
    {
        if (context.Topics.Any()) return;

        var math = context.Subjects.FirstOrDefault(s => s.Name == "Math");
        var reading = context.Subjects.FirstOrDefault(s => s.Name == "Reading");
        var writing = context.Subjects.FirstOrDefault(s => s.Name == "Writing");

        var topics = new List<Topic>();

        if (math is not null)
        {
            topics.Add(new Topic { Name = "Algebra", SubjectId = math.SubjectId });
            topics.Add(new Topic { Name = "Geometry", SubjectId = math.SubjectId });
        }

        if (reading is not null)
        {
            topics.Add(new Topic { Name = "Comprehension", SubjectId = reading.SubjectId });
            topics.Add(new Topic { Name = "Passage Mapping", SubjectId = reading.SubjectId });
        }

        if (writing is not null)
        {
            topics.Add(new Topic { Name = "Grammar", SubjectId = writing.SubjectId });
            // Replace 'Essay' with SAT-relevant writing topic: 'Conventions' (Standard English Conventions)
            topics.Add(new Topic { Name = "Conventions", SubjectId = writing.SubjectId });
        }

        if (topics.Any())
        {
            await context.Topics.AddRangeAsync(topics);
            await context.SaveChangesAsync();
        }
    }
}
