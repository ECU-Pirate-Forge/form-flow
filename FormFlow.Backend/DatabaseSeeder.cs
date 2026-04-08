using LiteDB;
using Microsoft.AspNetCore.Hosting;
using FormFlow.Backend;
//using FormFlow.Blazor.wwwroot;
using System.Collections.ObjectModel;
using FormFlow.Data.Models;

namespace FormFlow.Backend
{
    public class DatabaseSeeder
    {
        private readonly LiteDbContext _dbContext;
        private readonly IWebHostEnvironment _env;

        public DatabaseSeeder(LiteDbContext dbContext, IWebHostEnvironment env)
        {
            _dbContext = dbContext;
            _env = env;
        }

        // public void Seed()
        // {
        //     var questionDefinitionsCollection = _dbContext.GetCollection<QuestionDefinition>("question_definitions");

        //     if (questionDefinitionsCollection.Count() == 0)
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

        //         questionDefinitionsCollection.InsertBulk(questionDefinitions);
        //     }
        // }
    }

   
}