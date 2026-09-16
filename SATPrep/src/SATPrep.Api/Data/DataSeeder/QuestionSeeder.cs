using SATPrep.Api.Entities;

namespace SATPrep.Api.Data.DataSeeder;

public static class QuestionSeeder
{
    public static async Task Seed(AppDbContext context)
    {
        if (context.Questions.Any()) return;

        // Get topics for all 8 SAT domains
        var craftAndStructure = context.Topics.FirstOrDefault(t => t.Name == "Craft and Structure");
        var informationAndIdeas = context.Topics.FirstOrDefault(t => t.Name == "Information and Ideas");
        var standardEnglishConventions = context.Topics.FirstOrDefault(t => t.Name == "Standard English Conventions");
        var expressionOfIdeas = context.Topics.FirstOrDefault(t => t.Name == "Expression of Ideas");
        var algebra = context.Topics.FirstOrDefault(t => t.Name == "Algebra");
        var advancedMath = context.Topics.FirstOrDefault(t => t.Name == "Advanced Math");
        var geometryAndTrigonometry = context.Topics.FirstOrDefault(t => t.Name == "Geometry and Trigonometry");
        var problemSolvingDataAnalysis = context.Topics.FirstOrDefault(t => t.Name == "Problem-Solving and Data Analysis");

        // Get tags
        var tagCraftAndStructure = context.Tags.FirstOrDefault(t => t.Name == "craft-and-structure");
        var tagInformationAndIdeas = context.Tags.FirstOrDefault(t => t.Name == "information-and-ideas");
        var tagStandardEnglishConventions = context.Tags.FirstOrDefault(t => t.Name == "standard-english-conventions");
        var tagExpressionOfIdeas = context.Tags.FirstOrDefault(t => t.Name == "expression-of-ideas");
        var tagAlgebra = context.Tags.FirstOrDefault(t => t.Name == "algebra");
        var tagAdvancedMath = context.Tags.FirstOrDefault(t => t.Name == "advanced-math");
        var tagGeometryAndTrigonometry = context.Tags.FirstOrDefault(t => t.Name == "geometry-and-trigonometry");
        var tagProblemSolvingDataAnalysis = context.Tags.FirstOrDefault(t => t.Name == "problem-solving-data-analysis");

        var questions = new List<Question>();

        // === READING AND WRITING DOMAIN QUESTIONS ===

        // Craft and Structure
        if (craftAndStructure is not null)
        {
            var q1 = new Question
            {
                TopicId = craftAndStructure.TopicId,
                Text = @"""Why did you leave without saying goodbye?"" Maria asked, her voice trembling with unshed tears.
""Because I knew if I stayed, I couldn't let you go,"" he replied softly.

The novelist primarily uses dialogue in this passage to:",
                Difficulty = Difficulty.Medium
            };
            q1.Choices.Add(new Choice { Text = "establish the power dynamics in their relationship", IsCorrect = false });
            q1.Choices.Add(new Choice { Text = "reveal the emotional stakes of the characters' separation", IsCorrect = true });
            q1.Choices.Add(new Choice { Text = "demonstrate the author's skill with realistic speech", IsCorrect = false });
            q1.Choices.Add(new Choice { Text = "provide exposition about prior events", IsCorrect = false });
            questions.Add(q1);
        }

        // Information and Ideas
        if (informationAndIdeas is not null)
        {
            var q2 = new Question
            {
                TopicId = informationAndIdeas.TopicId,
                Text = @"During the eclipse, ancient peoples interpreted the Moon's shadow as a sign of divine anger. Modern astronomy has revealed that eclipses are purely mechanical phenomena, yet many still experience anxiety during these events.

The passage suggests that the main character's fear stems primarily from:",
                Difficulty = Difficulty.Medium
            };
            q2.Choices.Add(new Choice { Text = "misunderstanding of scientific concepts", IsCorrect = false });
            q2.Choices.Add(new Choice { Text = "lingering cultural memories of historical interpretations", IsCorrect = true });
            q2.Choices.Add(new Choice { Text = "inability to accept rational explanations", IsCorrect = false });
            q2.Choices.Add(new Choice { Text = "the purely mechanical nature of celestial events", IsCorrect = false });
            questions.Add(q2);
        }

        // Standard English Conventions
        if (standardEnglishConventions is not null)
        {
            var q3 = new Question
            {
                TopicId = standardEnglishConventions.TopicId,
                Text = @"The research team _____ decided to postpone the field expedition until weather conditions improve in late spring.

Which choice completes the text with correct grammar?",
                Difficulty = Difficulty.Easy
            };
            q3.Choices.Add(new Choice { Text = "have", IsCorrect = false });
            q3.Choices.Add(new Choice { Text = "has", IsCorrect = true });
            q3.Choices.Add(new Choice { Text = "are", IsCorrect = false });
            q3.Choices.Add(new Choice { Text = "were", IsCorrect = false });
            questions.Add(q3);
        }

        // Expression of Ideas
        if (expressionOfIdeas is not null)
        {
            var q4 = new Question
            {
                TopicId = expressionOfIdeas.TopicId,
                Text = @"Solar energy technology has evolved dramatically over the past two decades. [____] Consequently, utility companies worldwide are retiring coal infrastructure in favor of photovoltaic solar farms.

Which sentence most effectively develops the main argument presented in the passage?",
                Difficulty = Difficulty.Hard
            };
            q4.Choices.Add(new Choice { Text = "Solar panels are popular and come in various sizes.", IsCorrect = false });
            q4.Choices.Add(new Choice { Text = "The declining cost of production has made solar power economically competitive with fossil fuels.", IsCorrect = true });
            q4.Choices.Add(new Choice { Text = "Some consumers prefer solar panels while others favor hydroelectric dams.", IsCorrect = false });
            q4.Choices.Add(new Choice { Text = "Solar panels can be installed on private rooftops or in large solar fields.", IsCorrect = false });
            questions.Add(q4);
        }

        // === MATH DOMAIN QUESTIONS ===

        // Algebra
        if (algebra is not null)
        {
            var q5 = new Question
            {
                TopicId = algebra.TopicId,
                Text = @"If 3x + 7 = 22, what is the value of x?",
                Difficulty = Difficulty.Easy
            };
            q5.Choices.Add(new Choice { Text = "5", IsCorrect = true });
            q5.Choices.Add(new Choice { Text = "9.67", IsCorrect = false });
            q5.Choices.Add(new Choice { Text = "15", IsCorrect = false });
            q5.Choices.Add(new Choice { Text = "29", IsCorrect = false });
            questions.Add(q5);
        }

        // Geometry and Trigonometry
        if (geometryAndTrigonometry is not null)
        {
            var q6 = new Question
            {
                TopicId = geometryAndTrigonometry.TopicId,
                Text = @"In a right triangle, if one acute angle measures 35°, what is the measure of the other acute angle in degrees?",
                Difficulty = Difficulty.Easy
            };
            q6.Choices.Add(new Choice { Text = "35°", IsCorrect = false });
            q6.Choices.Add(new Choice { Text = "55°", IsCorrect = true });
            q6.Choices.Add(new Choice { Text = "90°", IsCorrect = false });
            q6.Choices.Add(new Choice { Text = "145°", IsCorrect = false });
            questions.Add(q6);
        }

        // Advanced Math
        if (advancedMath is not null)
        {
            var q7 = new Question
            {
                TopicId = advancedMath.TopicId,
                Text = @"The function f is defined by f(x) = x² + 3x - 5. What is the value of f(2)?",
                Difficulty = Difficulty.Medium
            };
            q7.Choices.Add(new Choice { Text = "-3", IsCorrect = false });
            q7.Choices.Add(new Choice { Text = "5", IsCorrect = false });
            q7.Choices.Add(new Choice { Text = "9", IsCorrect = true });
            q7.Choices.Add(new Choice { Text = "13", IsCorrect = false });
            questions.Add(q7);
        }

        // Problem-Solving and Data Analysis
        if (problemSolvingDataAnalysis is not null)
        {
            var q8 = new Question
            {
                TopicId = problemSolvingDataAnalysis.TopicId,
                Text = @"A university bookstore sold 150 textbooks in January, 200 textbooks in February, and 180 textbooks in March. What is the average (arithmetic mean) number of textbooks sold per month over these three months?",
                Difficulty = Difficulty.Easy
            };
            q8.Choices.Add(new Choice { Text = "143", IsCorrect = false });
            q8.Choices.Add(new Choice { Text = "150", IsCorrect = false });
            q8.Choices.Add(new Choice { Text = "176.7", IsCorrect = true });
            q8.Choices.Add(new Choice { Text = "530", IsCorrect = false });
            questions.Add(q8);
        }

        if (questions.Any())
        {
            await context.Questions.AddRangeAsync(questions);
            await context.SaveChangesAsync();

            // Attach tags based on question topic
            var addedQuestions = context.Questions.ToList();
            foreach (var q in addedQuestions)
            {
                if (q.TopicId == craftAndStructure?.TopicId && tagCraftAndStructure is not null)
                    context.QuestionTags.Add(new QuestionTag { QuestionId = q.QuestionId, TagId = tagCraftAndStructure.TagId });
                else if (q.TopicId == informationAndIdeas?.TopicId && tagInformationAndIdeas is not null)
                    context.QuestionTags.Add(new QuestionTag { QuestionId = q.QuestionId, TagId = tagInformationAndIdeas.TagId });
                else if (q.TopicId == standardEnglishConventions?.TopicId && tagStandardEnglishConventions is not null)
                    context.QuestionTags.Add(new QuestionTag { QuestionId = q.QuestionId, TagId = tagStandardEnglishConventions.TagId });
                else if (q.TopicId == expressionOfIdeas?.TopicId && tagExpressionOfIdeas is not null)
                    context.QuestionTags.Add(new QuestionTag { QuestionId = q.QuestionId, TagId = tagExpressionOfIdeas.TagId });
                else if (q.TopicId == algebra?.TopicId && tagAlgebra is not null)
                    context.QuestionTags.Add(new QuestionTag { QuestionId = q.QuestionId, TagId = tagAlgebra.TagId });
                else if (q.TopicId == advancedMath?.TopicId && tagAdvancedMath is not null)
                    context.QuestionTags.Add(new QuestionTag { QuestionId = q.QuestionId, TagId = tagAdvancedMath.TagId });
                else if (q.TopicId == geometryAndTrigonometry?.TopicId && tagGeometryAndTrigonometry is not null)
                    context.QuestionTags.Add(new QuestionTag { QuestionId = q.QuestionId, TagId = tagGeometryAndTrigonometry.TagId });
                else if (q.TopicId == problemSolvingDataAnalysis?.TopicId && tagProblemSolvingDataAnalysis is not null)
                    context.QuestionTags.Add(new QuestionTag { QuestionId = q.QuestionId, TagId = tagProblemSolvingDataAnalysis.TagId });
            }

            await context.SaveChangesAsync();
        }
    }
}
