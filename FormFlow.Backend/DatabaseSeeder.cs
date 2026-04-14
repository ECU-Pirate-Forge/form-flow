using LiteDB;
using System.Text.Json;
using FormFlow.Data.Models;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace FormFlow.Backend
{
    public class DatabaseSeeder
    {
        private readonly ILiteDatabase _dbContext;
        private readonly IWebHostEnvironment _env;

        public DatabaseSeeder(ILiteDatabase dbContext, IWebHostEnvironment env)
        {
            _dbContext = dbContext;
            _env = env;
        }
        // public void SeedInLine(ILiteCollection<QuestionDefinition> collection)
        // {
        //     //var questionDefinitionsCollection = _dbContext.GetCollection<QuestionDefinition>("question_definitions");
        //     if (collection.Count() == 0)
        //     {
        //         var questionDefinitions = new List<QuestionDefinition>
        //         {
        //             new QuestionDefinition
        //             {
        //                 Id = Guid.NewGuid(),
        //                 Key = "first_name",
        //                 Label = "First Name",
        //                 Type = "text",
        //                 Required = true,
        //                 Placeholder = "Enter your first name"
        //             },
        //             new QuestionDefinition
        //             {
        //                 Id = Guid.NewGuid(),
        //                 Key = "last_name",
        //                 Label = "Last Name",
        //                 Type = "text",
        //                 Required = true,
        //                 Placeholder = "Enter your last name"
        //             },
        //             new QuestionDefinition
        //             {
        //                 Id = Guid.NewGuid(),
        //                 Key = "email",
        //                 Label = "Email Address",
        //                 Type = "email",
        //                 Required = true,
        //                 Placeholder = "Enter your email address"
        //             }
        //         };
        //         collection.InsertBulk(questionDefinitions);
        //     }
        // }
        public void SeedFromJson(ILiteCollection<QuestionDefinition> collection)
        {
            if (collection.Count() == 0)
            {
                var seedDataPath = Path.Combine(_env.ContentRootPath, "SeedData", "questions.json");

                if (!File.Exists(seedDataPath))
                {
                    throw new FileNotFoundException($"Seed data file not found: {seedDataPath}");
                }

                var json = File.ReadAllText(seedDataPath);
                var questionDefinitions = JsonSerializer.Deserialize<List<QuestionDefinition>>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (questionDefinitions == null || questionDefinitions.Count == 0)
                {
                    throw new InvalidDataException($"Seed data file is empty or could not be deserialized: {seedDataPath}");
                }

                foreach (var question in questionDefinitions)
                {
                    if (question.Id == Guid.Empty)
                    {
                        question.Id = Guid.NewGuid();
                    }
                }

                collection.InsertBulk(questionDefinitions);
            }
        }
    }
}