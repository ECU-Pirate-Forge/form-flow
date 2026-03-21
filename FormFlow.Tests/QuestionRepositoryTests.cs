using System;
using System.IO;

using FormFlow.Backend.Repositories;

using LiteDB;

using Xunit;

namespace FormFlow.Tests
{
    public class QuestionRepositoryTests
    {
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
    }
}