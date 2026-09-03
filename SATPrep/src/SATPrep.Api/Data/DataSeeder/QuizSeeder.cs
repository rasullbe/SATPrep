using SATPrep.Api.Entities;

namespace SATPrep.Api.Data.DataSeeder;

public static class QuizSeeder
{
    public static async Task Seed(AppDbContext context)
    {
        if (context.Quizzes.Any()) return;

        var creator = context.Users.FirstOrDefault();
        var questions = context.Questions.Take(5).ToList();

        if (creator is null || !questions.Any()) return;

        var quiz = new Quiz
        {
            Title = "Sample Math Quiz",
            Description = "A short sample quiz.",
            CreatedById = creator.UserId,
            CreatedAt = DateTime.UtcNow
        };

        await context.Quizzes.AddAsync(quiz);
        await context.SaveChangesAsync();

        // attach up to 5 questions
        var order = 1;
        foreach (var q in questions)
        {
            context.QuizQuestions.Add(new QuizQuestion { QuizId = quiz.QuizId, QuestionId = q.QuestionId, Order = order++ });
        }

        await context.SaveChangesAsync();
    }
}
