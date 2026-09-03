namespace SATPrep.Api.Data.DataSeeder;

public static class DataSeeder
{
    public static async Task SeedAllAsync(AppDbContext context)
    {
        // ensure order: users -> subjects -> topics -> tags -> questions -> quizzes -> flashcards -> attempts
        await UserSeeder.Seed(context);
        await SubjectSeeder.Seed(context);
        await TopicSeeder.Seed(context);
        await TagSeeder.Seed(context);
        await QuestionSeeder.Seed(context);
        await QuizSeeder.Seed(context);
        await FlashcardSeeder.Seed(context);
        await QuizAttemptSeeder.Seed(context);
    }
}
