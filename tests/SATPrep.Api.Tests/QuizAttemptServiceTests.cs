using Microsoft.EntityFrameworkCore;
using SATPrep.Api.Data;
using SATPrep.Api.DTOs;
using SATPrep.Api.Entities;
using SATPrep.Api.Repositories;
using SATPrep.Api.Services;
using SATPrep.Api.Tests.Fixtures;

namespace SATPrep.Api.Tests;

public class QuizAttemptServiceTests
{
    private readonly TestDatabaseFixture _fixture = new();

    [Fact]
    public async Task StartAsync_ReturnsQuizAttempt_WhenQuizExists()
    {
        var context = _fixture.GetDbContext();
        var service = CreateService(context);
        var (userId, quizId, _, _) = await SeedQuizAsync(context);

        var result = await service.StartAsync(userId, quizId);

        Assert.NotNull(result);
        Assert.Equal(2, result.TotalQuestions);
        Assert.Equal(0, result.PointsCorrect);
    }

    [Fact]
    public async Task CompleteAsync_GradesCorrectly_SetsPointsAndScore()
    {
        var context = _fixture.GetDbContext();
        var service = CreateService(context);
        var (userId, quizId, q1, q2) = await SeedQuizAsync(context);

        var attempt = await service.StartAsync(userId, quizId);
        Assert.NotNull(attempt);

        var correctChoice = q1.Choices.First(c => c.IsCorrect);
        var wrongChoice = q2.Choices.First(c => !c.IsCorrect);

        var result = await service.CompleteAsync(attempt.QuizAttemptId, userId, new CompleteQuizAttemptDto
        {
            TimeTakenSeconds = 180,
            Answers = new List<AnswerCreateDto>
            {
                new() { QuestionId = q1.QuestionId, SelectedChoiceId = correctChoice.ChoiceId },
                new() { QuestionId = q2.QuestionId, SelectedChoiceId = wrongChoice.ChoiceId }
            }
        });

        Assert.NotNull(result);
        Assert.Equal(1, result.PointsCorrect);
        Assert.Equal(2, result.TotalQuestions);
        Assert.Equal(50, result.Score);

        var stored = await context.QuizAttempts
            .Include(a => a.Answers)
            .FirstOrDefaultAsync(a => a.QuizAttemptId == attempt.QuizAttemptId);
        Assert.NotNull(stored);
        Assert.Equal(AttemptStatus.Completed, stored.Status);
        Assert.Collection(stored.Answers,
            a => Assert.True(a.IsCorrect),
            a => Assert.False(a.IsCorrect));
    }

    private static QuizAttemptService CreateService(AppDbContext context)
    {
        return new QuizAttemptService(
            new QuizAttemptRepository(context),
            new QuizRepository(context),
            new QuestionRepository(context),
            new UserProgressRepository(context),
            new StudySessionRepository(context));
    }

    private static async Task<(long UserId, long QuizId, Question Q1, Question Q2)> SeedQuizAsync(AppDbContext context)
    {
        var user = new User
        {
            Name = "Quiz User",
            Email = "quiz@example.com",
            Password = "QuizPass1"
        };

        var subject = new Subject { Name = "Math" };
        var topic = new Topic { Name = "Algebra", Subject = subject };

        var q1 = new Question
        {
            Text = "What is 2 + 2?",
            Topic = topic,
            Difficulty = Difficulty.Easy,
            Choices = new List<Choice>
            {
                new() { Text = "4", IsCorrect = true },
                new() { Text = "5", IsCorrect = false }
            }
        };

        var q2 = new Question
        {
            Text = "What is 3 + 3?",
            Topic = topic,
            Difficulty = Difficulty.Easy,
            Choices = new List<Choice>
            {
                new() { Text = "6", IsCorrect = true },
                new() { Text = "7", IsCorrect = false }
            }
        };

        var quiz = new Quiz
        {
            Title = "Math Quiz",
            CreatedBy = user,
            QuizQuestions = new List<QuizQuestion>
            {
                new() { Question = q1, Order = 1 },
                new() { Question = q2, Order = 2 }
            }
        };

        context.Users.Add(user);
        context.Subjects.Add(subject);
        context.Topics.Add(topic);
        context.Questions.AddRange(q1, q2);
        context.Quizzes.Add(quiz);
        await context.SaveChangesAsync();

        return (user.UserId, quiz.QuizId, q1, q2);
    }
}