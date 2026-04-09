using LiteDB;
using System.Text.Json;
using FormFlow.Data.Models;

namespace FormFlow.Backend
{
    public class DatabaseSeeder
    {
        private readonly ILiteDatabase _dbContext;

        public DatabaseSeeder(ILiteDatabase dbContext, IWebHostEnvironment env)
        {
            _dbContext = dbContext;
        }
        public void SeedInLine(ILiteCollection<QuestionDefinition> collection)
        {
            //var questionDefinitionsCollection = _dbContext.GetCollection<QuestionDefinition>("question_definitions");
            if (collection.Count() == 0)
            {
                var questionDefinitions = new List<QuestionDefinition>
                {
                    new QuestionDefinition
                    {
                        Id = Guid.NewGuid(),
                        Key = "first_name",
                        Label = "First Name",
                        Type = "text",
                        Required = true,
                        Placeholder = "Enter your first name"
                    },
                    new QuestionDefinition
                    {
                        Id = Guid.NewGuid(),
                        Key = "last_name",
                        Label = "Last Name",
                        Type = "text",
                        Required = true,
                        Placeholder = "Enter your last name"
                    },
                    new QuestionDefinition
                    {
                        Id = Guid.NewGuid(),
                        Key = "email",
                        Label = "Email Address",
                        Type = "email",
                        Required = true,
                        Placeholder = "Enter your email address"
                    }
                };
                collection.InsertBulk(questionDefinitions);
            }
        }
    }
}