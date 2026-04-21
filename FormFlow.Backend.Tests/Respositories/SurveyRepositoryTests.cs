using System;
using FluentAssertions;
using LiteDB;
using Xunit;
using FormFlow.Data.Models;
using FormFlow.Backend.Repositories;

namespace FormFlow.Backend.Tests.Repositories
{
    public class SurveyRepositoryTests
    {
        private ILiteDatabase CreateInMemoryDb()
        {
            return new LiteDatabase("Filename=:memory:");
        }

        [Fact]
        public void Repository_Initializes_Surveys_Collection()
        {
            using var db = CreateInMemoryDb();
            var repo = new SurveyRepository(db);

            repo.Surveys.Should().NotBeNull();
            repo.Surveys.Name.Should().Be("surveys");
        }

        [Fact]
        public void Repository_Creates_Unique_Id_Index()
        {
            using var db = CreateInMemoryDb();
            var repo = new SurveyRepository(db);

            // Insert a survey
            var survey = new SurveyDefinition
            {
                Id = Guid.NewGuid(),
                Title = "Test Survey",
                Description = "desc",
                QuestionIds = new(),
                CreatedAt = DateTime.UtcNow
            };

            repo.Surveys.Insert(survey);

            // Attempt duplicate insert
            Action act = () => repo.Surveys.Insert(survey);

            act.Should().Throw<LiteException>();
        }

        [Fact]
        public void Repository_Can_Insert_And_Retrieve_Survey()
        {
            using var db = CreateInMemoryDb();
            var repo = new SurveyRepository(db);

            var survey = new SurveyDefinition
            {
                Id = Guid.NewGuid(),
                Title = "Customer Feedback",
                Description = "Basic survey",
                QuestionIds = [Guid.NewGuid(), Guid.NewGuid()],
                CreatedAt = DateTime.UtcNow
            };

            repo.Surveys.Insert(survey);

            var retrieved = repo.Surveys.FindById(survey.Id);

            retrieved.Should().NotBeNull();
            retrieved!.Id.Should().Be(survey.Id);
            retrieved.Title.Should().Be("Customer Feedback");
            retrieved.QuestionIds.Should().HaveCount(2);
        }

        [Fact]
        public void Repository_Returns_Null_For_Missing_Survey()
        {
            using var db = CreateInMemoryDb();
            var repo = new SurveyRepository(db);

            var result = repo.Surveys.FindById(Guid.NewGuid());

            result.Should().BeNull();
        }
    }
}
