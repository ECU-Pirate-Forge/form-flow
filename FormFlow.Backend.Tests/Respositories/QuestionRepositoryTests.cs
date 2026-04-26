using System;
using System.IO;
using System.Reflection.Emit;
using FluentAssertions;
using FormFlow.Backend.Repositories;
using FormFlow.Data.Models;
using LiteDB;

using Xunit;

namespace FormFlow.Tests
{
    public class QuestionRepositoryTests
    {
        private ILiteDatabase CreateInMemoryDatabase() => new LiteDatabase("Filename=:memory:");


        [Fact]
        public void Repository_Intializes_Questions_Collection()
        {
            // Arrange
            using var db = CreateInMemoryDatabase();

            // Act
            var repository = new QuestionRepository(db);

            // Assert
            repository.Questions.Should().NotBeNull();
            repository.Questions.Should().Be("questions");

        }
        [Fact]
        public void Repository_Creates_Unique_Id_Index()
        {

            using var db = CreateInMemoryDatabase();
            var repository = new QuestionRepository(db);

            var question = BuildQuestion();
            repository.Insert(question);

            Action act = () => repository.Questions.Insert(question);

            act.Should().Throw<LiteException>();

        }

        [Fact]
        public void Insert_Persists_Question_To_Collection()
        {
            using var db = CreateInMemoryDatabase();
            var repository = new QuestionRepository(db);

            var question = BuildQuestion(key: "SuperSecretKey", label: "Super Secret Label");
            repository.Insert(question);

            var retrieved = repository.Questions.FindById(question.Id);
            retrieved.Should().NotBeNull();
            retrieved!.Key.Should().Be("SuperSecretKey");
            retrieved.Label.Should().Be("Super Secret Label");
        }

        [Fact]
        public void Insert_Returns_The_Inserted_Question()
        {
            using var db = CreateInMemoryDatabase();
            var repository = new QuestionRepository(db);

            var question = BuildQuestion();
            var result = repository.Insert(question);

            result.Should().NotBeNull();
            result.Id.Should().Be(question.Id);
            result.Key.Should().Be(question.Key);
            result.Label.Should().Be(question.Label);
        }

        [Fact]
        public void Insert_Persists_Options_For_OptionBased_Types()
        {
            using var db = CreateInMemoryDatabase();
            var repository = new QuestionRepository(db);

            var question = BuildQuestion(type: "dropdown");
            question.Options = new List<Option>
            {
                new Option { Label = "Option 1", Value = "option1" },
                new Option { Label = "Option 2", Value = "option2" }
            };

            repository.Insert(question);

            var retrieved = repository.Questions.FindById(question.Id);
            retrieved!.Options.Should().HaveCount(2);
            retrieved.Options[0].Value.Should().Be("option1");
            retrieved.Options[1].Value.Should().Be("option2");
        }

        [Fact]
        public void FindById_Returns_Correct_Question()
        {
            using var db = CreateInMemoryDatabase();
            var repository = new QuestionRepository(db);

            var question = BuildQuestion(key: "age", label: "Age");
            repository.Insert(question);

            var result = repository.FindById(question.Id);

            result.Should().NotBeNull();
            result!.Id.Should().Be(question.Id);
            result.Key.Should().Be("age");
        }

        [Fact]
        public void FindById_Returns_Null_For_Unknown_Id()
        {
            using var db = CreateInMemoryDatabase();
            var repository = new QuestionRepository(db);

            var result = repository.FindById(Guid.NewGuid());

            result.Should().BeNull();
        }

        [Fact]
        public void FindAll_Returns_All_Inserted_Questions()
        {
            using var db = CreateInMemoryDatabase();
            var repository = new QuestionRepository(db);

            var question1 = BuildQuestion(key: "q1", label: "Question 1");
            var question2 = BuildQuestion(key: "q2", label: "Question 2");
            var question3 = BuildQuestion(key: "q3", label: " Question 3");
            repository.Insert(question1);
            repository.Insert(question2);
            repository.Insert(question3);


            var results = repository.FindAll().ToList();

            results.Should().HaveCount(3);
            results.Select(q => q.Key).Should().Contain(new[] { "q1", "q2", "q3" });

        }

        [Fact]
        public void FindAll_Returns_Empty_When_No_Questions_Exist()
        {
            using var db = CreateInMemoryDatabase();
            var repository = new QuestionRepository(db);

            var result = repository.FindAll().ToList();

            result.Should().BeEmpty();
        }

        [Fact]
        public void FindOne_Returns_Match_On_Perdicate()
        {
            using var db = CreateInMemoryDatabase();
            var repo = new QuestionRepository(db);

            repo.Insert(BuildQuestion(key: "e", label: "A letter"));
            repo.Insert(BuildQuestion(key: "r", label: "Another letter"));

            var result = repo.FindOne(q => q.Key == "r");

            result.Should().NotBeNull();
            result!.Label.Should().Be("Another letter");
        }

        [Fact]
        public void FindOne_Returns_Null_When_No_Match()
        {
            using var db = CreateInMemoryDatabase();
            var repo = new QuestionRepository(db);

            repo.Insert(BuildQuestion(key: "yin", label: "Yin"));

            var result = repo.FindOne(q => q.Key == "made up key");

            result.Should().BeNull();
        }




        private static QuestionDefinition BuildQuestion(
            string? key = "test_key",
            string? label = "Test Label",
            string type = "text") => new()
            {
                Id = Guid.NewGuid(),
                Key = key,
                Label = label,
                Type = type,
                Required = true

            };
    }
}