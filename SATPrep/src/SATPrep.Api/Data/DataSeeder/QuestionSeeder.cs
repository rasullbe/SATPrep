using SATPrep.Api.Entities;

namespace SATPrep.Api.Data.DataSeeder;

public static class QuestionSeeder
{
    public static async Task Seed(AppDbContext context)
    {
        if (context.Questions.Any()) return;

        var algebra = context.Topics.FirstOrDefault(t => t.Name == "Algebra");
        var grammar = context.Topics.FirstOrDefault(t => t.Name == "Grammar");

        var tagAlgebra = context.Tags.FirstOrDefault(t => t.Name == "algebra");
        var tagGrammar = context.Tags.FirstOrDefault(t => t.Name == "grammar");

        var questions = new List<Question>();

        if (algebra is not null)
        {
            var q1 = new Question
            {
                TopicId = algebra.TopicId,
                Text = "What is 2 + 2?",
                Difficulty = Difficulty.Easy
            };

            q1.Choices.Add(new Choice { Text = "3", IsCorrect = false });
            q1.Choices.Add(new Choice { Text = "4", IsCorrect = true });
            q1.Choices.Add(new Choice { Text = "5", IsCorrect = false });

            questions.Add(q1);
        }

        if (grammar is not null)
        {
            var q2 = new Question
            {
                TopicId = grammar.TopicId,
                Text = "Choose the correctly punctuated sentence.",
                Difficulty = Difficulty.Medium
            };

            q2.Choices.Add(new Choice { Text = "Its raining.", IsCorrect = false });
            q2.Choices.Add(new Choice { Text = "It's raining.", IsCorrect = true });
            q2.Choices.Add(new Choice { Text = "Its' raining.", IsCorrect = false });

            questions.Add(q2);
        }

        if (questions.Any())
        {
            await context.Questions.AddRangeAsync(questions);
            await context.SaveChangesAsync();

            // Attach tags where possible
            var addedQuestions = context.Questions.ToList();
            foreach (var q in addedQuestions)
            {
                if (q.Text.Contains("2 + 2") && tagAlgebra is not null)
                {
                    context.QuestionTags.Add(new QuestionTag { QuestionId = q.QuestionId, TagId = tagAlgebra.TagId });
                }

                if (q.Text.Contains("punctuated") && tagGrammar is not null)
                {
                    context.QuestionTags.Add(new QuestionTag { QuestionId = q.QuestionId, TagId = tagGrammar.TagId });
                }
            }

            await context.SaveChangesAsync();
        }
    }
}
