using Microsoft.EntityFrameworkCore;
using SATPrep.Api.Entities;
namespace SATPrep.Api.Data;

public class AppDbContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<Quiz> Quizzes { get; set; }
    public DbSet<Question> Questions { get; set; }
    public DbSet<Subject> Subjects { get; set; }
    public DbSet<Topic> Topics { get; set; }
    public DbSet<QuizQuestion> QuizQuestions { get; set; }
    public DbSet<Answer> Answers { get; set; }
    public DbSet<Choice> Choices { get; set; }
    public DbSet<Flashcard> Flashcards { get; set; }
    public DbSet<QuizAttempt> QuizAttempts { get; set; }
    public DbSet<Tag> Tags { get; set; }
    public DbSet<QuestionTag> QuestionTags { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(AppDbContext).Assembly);

        base.OnModelCreating(modelBuilder);
    }
}
