using Microsoft.EntityFrameworkCore;
using SATPrep.Api.Entities;

namespace SATPrep.Api.Data.DataSeeder;

public static class QuizSeeder
{
    public static async Task Seed(AppDbContext context)
    {
        if (context.Quizzes.Any()) return;

        var creator = context.Users.FirstOrDefault();
        var allQuestions = context.Questions.ToList();

        if (creator is null || !allQuestions.Any()) return;

        // Full-length diagnostic (representative of SAT structure)
        var quiz1 = new Quiz
        {
            Title = "Digital SAT Full Practice Test",
            Description = "Complete practice exam with Reading and Writing + Math sections",
            CreatedById = creator.UserId,
            CreatedAt = DateTime.UtcNow
        };

        // Section-specific quizzes
        var quiz2 = new Quiz
        {
            Title = "Reading and Writing - Module 1",
            Description = "Reading and Writing practice module with mixed domains",
            CreatedById = creator.UserId,
            CreatedAt = DateTime.UtcNow
        };

        var quiz3 = new Quiz
        {
            Title = "Math - Domain Practice: Algebra",
            Description = "Focused algebra problem set",
            CreatedById = creator.UserId,
            CreatedAt = DateTime.UtcNow
        };

        await context.Quizzes.AddAsync(quiz1);
        await context.Quizzes.AddAsync(quiz2);
        await context.Quizzes.AddAsync(quiz3);
        await context.SaveChangesAsync();

        // Attach questions to quizzes
        var order = 1;

        // Full test: all questions
        foreach (var q in allQuestions)
        {
            context.QuizQuestions.Add(new QuizQuestion { QuizId = quiz1.QuizId, QuestionId = q.QuestionId, Order = order++ });
        }

        // RW Module: first 4 questions (RW domain questions)
        order = 1;
        var rwQuestions = allQuestions.Take(4).ToList();
        foreach (var q in rwQuestions)
        {
            context.QuizQuestions.Add(new QuizQuestion { QuizId = quiz2.QuizId, QuestionId = q.QuestionId, Order = order++ });
        }

        // Math Algebra: algebra question only
        order = 1;
        var algebraQuestion = allQuestions.FirstOrDefault(q => q.Topic.Name == "Algebra");
        if (algebraQuestion is not null)
        {
            context.QuizQuestions.Add(new QuizQuestion { QuizId = quiz3.QuizId, QuestionId = algebraQuestion.QuestionId, Order = order });
        }

        await context.SaveChangesAsync();
    }
}
