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

        // Craft and Structure - dialogue analysis
        if (craftAndStructure is not null)
        {
            var q1 = new Question
            {
                TopicId = craftAndStructure.TopicId,
                Text = @"The novelist primarily uses dialogue in this passage to:

""Why did you leave without saying goodbye?"" Maria asked, her voice trembling with unshed tears.
""Because I knew if I stayed, I couldn't let you go,"" he replied softly.

(A) establish the power dynamics in their relationship
(B) reveal the emotional stakes of the characters' separation
(C) demonstrate the author's skill with realistic speech
(D) provide exposition about prior events",
                Difficulty = Difficulty.Medium
            };
            q1.Choices.Add(new Choice { Text = "(A) establish the power dynamics in their relationship", IsCorrect = false });
            q1.Choices.Add(new Choice { Text = "(B) reveal the emotional stakes of the characters' separation", IsCorrect = true });
            q1.Choices.Add(new Choice { Text = "(C) demonstrate the author's skill with realistic speech", IsCorrect = false });
            q1.Choices.Add(new Choice { Text = "(D) provide exposition about prior events", IsCorrect = false });
            questions.Add(q1);
        }

        // Information and Ideas - comprehension and inference
        if (informationAndIdeas is not null)
        {
            var q2 = new Question
            {
                TopicId = informationAndIdeas.TopicId,
                Text = @"The passage suggests that the main character's fear stems primarily from:

During the eclipse, ancient peoples interpreted the Moon's shadow as a sign of divine anger. Modern astronomy has revealed that eclipses are purely mechanical phenomena, yet many still experience anxiety during these events.

(A) misunderstanding of scientific concepts
(B) lingering cultural memories of historical interpretations
(C) inability to accept rational explanations
(D) the purely mechanical nature of celestial events",
                Difficulty = Difficulty.Medium
            };
            q2.Choices.Add(new Choice { Text = "(A) misunderstanding of scientific concepts", IsCorrect = false });
            q2.Choices.Add(new Choice { Text = "(B) lingering cultural memories of historical interpretations", IsCorrect = true });
            q2.Choices.Add(new Choice { Text = "(C) inability to accept rational explanations", IsCorrect = false });
            q2.Choices.Add(new Choice { Text = "(D) the purely mechanical nature of celestial events", IsCorrect = false });
            questions.Add(q2);
        }

        // Standard English Conventions - subject-verb agreement
        if (standardEnglishConventions is not null)
        {
            var q3 = new Question
            {
                TopicId = standardEnglishConventions.TopicId,
                Text = @"Which choice completes the sentence with correct grammar?

The team _____ decided to postpone the match until next week.

(A) have
(B) has
(C) are
(D) were",
                Difficulty = Difficulty.Easy
            };
            q3.Choices.Add(new Choice { Text = "(A) have", IsCorrect = false });
            q3.Choices.Add(new Choice { Text = "(B) has", IsCorrect = true });
            q3.Choices.Add(new Choice { Text = "(C) are", IsCorrect = false });
            q3.Choices.Add(new Choice { Text = "(D) were", IsCorrect = false });
            questions.Add(q3);
        }

        // Expression of Ideas - editing for rhetoric and flow
        if (expressionOfIdeas is not null)
        {
            var q4 = new Question
            {
                TopicId = expressionOfIdeas.TopicId,
                Text = @"Which sentence most effectively develops the main argument presented in the passage about renewable energy adoption?

Current text: ""Solar panels are popular. They work by converting sunlight to electricity.""

(A) Solar panels are popular and come in various colors.
(B) The declining cost of solar technology has made renewable energy economically competitive with fossil fuels in many markets.
(C) Some people prefer solar panels while others prefer wind energy.
(D) Solar panels can be installed on rooftops or in large arrays.",
                Difficulty = Difficulty.Hard
            };
            q4.Choices.Add(new Choice { Text = "(A) Solar panels are popular and come in various colors.", IsCorrect = false });
            q4.Choices.Add(new Choice { Text = "(B) The declining cost of solar technology has made renewable energy economically competitive with fossil fuels in many markets.", IsCorrect = true });
            q4.Choices.Add(new Choice { Text = "(C) Some people prefer solar panels while others prefer wind energy.", IsCorrect = false });
            q4.Choices.Add(new Choice { Text = "(D) Solar panels can be installed on rooftops or in large arrays.", IsCorrect = false });
            questions.Add(q4);
        }

        // === MATH DOMAIN QUESTIONS ===

        // Algebra
        if (algebra is not null)
        {
            var q5 = new Question
            {
                TopicId = algebra.TopicId,
                Text = @"If 3x + 7 = 22, what is the value of x?

(A) 5
(B) 9.67
(C) 15
(D) 29",
                Difficulty = Difficulty.Easy
            };
            q5.Choices.Add(new Choice { Text = "(A) 5", IsCorrect = true });
            q5.Choices.Add(new Choice { Text = "(B) 9.67", IsCorrect = false });
            q5.Choices.Add(new Choice { Text = "(C) 15", IsCorrect = false });
            q5.Choices.Add(new Choice { Text = "(D) 29", IsCorrect = false });
            questions.Add(q5);
        }

        // Geometry and Trigonometry
        if (geometryAndTrigonometry is not null)
        {
            var q6 = new Question
            {
                TopicId = geometryAndTrigonometry.TopicId,
                Text = @"In a right triangle, if one acute angle measures 35°, what is the measure of the other acute angle?

(A) 35°
(B) 55°
(C) 90°
(D) 145°",
                Difficulty = Difficulty.Easy
            };
            q6.Choices.Add(new Choice { Text = "(A) 35°", IsCorrect = false });
            q6.Choices.Add(new Choice { Text = "(B) 55°", IsCorrect = true });
            q6.Choices.Add(new Choice { Text = "(C) 90°", IsCorrect = false });
            q6.Choices.Add(new Choice { Text = "(D) 145°", IsCorrect = false });
            questions.Add(q6);
        }

        // Advanced Math
        if (advancedMath is not null)
        {
            var q7 = new Question
            {
                TopicId = advancedMath.TopicId,
                Text = @"If f(x) = x² + 3x - 5, what is f(2)?

(A) -3
(B) 5
(C) 9
(D) 13",
                Difficulty = Difficulty.Medium
            };
            q7.Choices.Add(new Choice { Text = "(A) -3", IsCorrect = false });
            q7.Choices.Add(new Choice { Text = "(B) 5", IsCorrect = false });
            q7.Choices.Add(new Choice { Text = "(C) 9", IsCorrect = true });
            q7.Choices.Add(new Choice { Text = "(D) 13", IsCorrect = false });
            questions.Add(q7);
        }

        // Problem-Solving and Data Analysis
        if (problemSolvingDataAnalysis is not null)
        {
            var q8 = new Question
            {
                TopicId = problemSolvingDataAnalysis.TopicId,
                Text = @"A store sold 150 items in January, 200 items in February, and 180 items in March. What is the average number of items sold per month over these three months?

(A) 143
(B) 150
(C) 176.67
(D) 530",
                Difficulty = Difficulty.Easy
            };
            q8.Choices.Add(new Choice { Text = "(A) 143", IsCorrect = false });
            q8.Choices.Add(new Choice { Text = "(B) 150", IsCorrect = false });
            q8.Choices.Add(new Choice { Text = "(C) 176.67", IsCorrect = true });
            q8.Choices.Add(new Choice { Text = "(D) 530", IsCorrect = false });
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
