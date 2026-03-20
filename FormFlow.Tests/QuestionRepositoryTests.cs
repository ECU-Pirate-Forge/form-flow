using System;
using System.IO;

using FormFlow.Backend.Repositories;
using FormFlow.Data.Models;

using LiteDB;

using Xunit;

namespace FormFlow.Tests
{
    public class QuestionRepositoryTests : IClassFixture<BackendFactory>
    {
        private readonly BackendFactory _factory;

        public QuestionRepositoryTests(BackendFactory factory)
        {
            _factory = factory;
        }

        [Fact]
        public void Repository_ExposesQuestionsCollection_WithExpectedName()
        {
            var dbPath = Path.Combine(Path.GetTempPath(), $"formflow-repo-test-{Guid.NewGuid():N}.db");

            try
            {
                using var database = new LiteDatabase($"Filename={dbPath};Connection=shared");
                var repository = new QuestionRepository(database);

                Assert.Equal("questions", repository.Questions.Name);
            }
            finally
            {
                if (File.Exists(dbPath))
                {
                    File.Delete(dbPath);
                }
            }
        }

        [Fact]
        public void Repository_AllowsRoundTrip_ById()
        {
            var dbPath = Path.Combine(Path.GetTempPath(), $"formflow-repo-test-{Guid.NewGuid():N}.db");

            try
            {
                using var database = new LiteDatabase($"Filename={dbPath};Connection=shared");
                var repository = new QuestionRepository(database);

                var id = Guid.NewGuid();
                var question = new QuestionDefinition
                {
                    Id = id,
                    Key = "favorite_color",
                    Label = "What is your favorite color?",
                    Type = "text",
                    Required = true,
                    Placeholder = "Type a color",
                    DefaultValue = string.Empty,
                    Options = new(),
                    VisibleIf = default,
                    ValidationRules = new ValidationRules { MinLength = 1, MaxLength = 50 },
                    HelpText = "Enter one color"
                };

                repository.Questions.Insert(question);

                var fromDb = repository.Questions.FindById(id);

                Assert.NotNull(fromDb);
                Assert.Equal(id, fromDb!.Id);
                Assert.Equal(question.Key, fromDb.Key);
            }
            finally
            {
                if (File.Exists(dbPath))
                {
                    File.Delete(dbPath);
                }
            }
        }

        [Fact]
        public void DependencyInjection_ResolvesQuestionRepository_AsSingleton()
        {
            var first = _factory.Services.GetService(typeof(IQuestionRepository));
            var second = _factory.Services.GetService(typeof(IQuestionRepository));

            Assert.NotNull(first);
            Assert.NotNull(second);
            Assert.Same(first, second);
        }
    }
}