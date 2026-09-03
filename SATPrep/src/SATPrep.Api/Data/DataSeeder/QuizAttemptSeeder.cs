using Microsoft.EntityFrameworkCore;
using SATPrep.Api.Entities;

namespace SATPrep.Api.Data.DataSeeder;

public static class QuizAttemptSeeder
{
    public static async Task Seed(AppDbContext context)
    {
        if (context.QuizAttempts.Any()) return;

        var user = context.Users.FirstOrDefault();
        var quiz = context.Quizzes.Include(q => q.QuizQuestions).FirstOrDefault();
        var question = context.Questions.Include(q => q.Choices).FirstOrDefault();

        if (user is null || quiz is null || question is null) return;

        var attempt = new QuizAttempt
        {
            QuizId = quiz.QuizId,
            UserId = user.UserId,
            StartedAt = DateTime.UtcNow,
            CompletedAt = DateTime.UtcNow,
            Score = 0
        };

        await context.QuizAttempts.AddAsync(attempt);
        await context.SaveChangesAsync();

        // create one answer using the first correct choice if available
        var correctChoice = question.Choices.FirstOrDefault(c => c.IsCorrect);
        var ans = new Answer
        {
            QuizAttemptId = attempt.QuizAttemptId,
            QuestionId = question.QuestionId,
            SelectedChoiceId = correctChoice?.ChoiceId,
            IsCorrect = correctChoice is not null && correctChoice.IsCorrect,
            AnsweredAt = DateTime.UtcNow
        };

        await context.Answers.AddAsync(ans);

        // simple score calculation
        attempt.Score = ans.IsCorrect ? 1 : 0;

        await context.SaveChangesAsync();
    }
}
