using FluentAssertions;
using LiteDB;
using Moq;
using FormFlow.Backend;
using FormFlow.Data.Models;
using System.Text.Json;
using Microsoft.AspNetCore.Hosting;

namespace FormFlow.Backend.Tests;

public class DatabaseSeederTests
{
    [Fact]
    public void SeedInLine_WithEmptyCollection_InsertsExpectedNumberOfDocuments()
    {
        // Arrange
        var mockDatabase = new Mock<ILiteDatabase>();
        var mockCollection = new Mock<ILiteCollection<QuestionDefinition>>();
        var mockEnv = new Mock<IWebHostEnvironment>();

        mockDatabase
            .Setup(db => db.GetCollection<QuestionDefinition>("question_definitions"))
            .Returns(mockCollection.Object);

        mockCollection.Setup(c => c.Count()).Returns(0);
        mockCollection.Setup(c => c.InsertBulk(It.IsAny<IEnumerable<QuestionDefinition>>()))
            .Returns(3);

        var seeder = new DatabaseSeeder(mockDatabase.Object, mockEnv.Object);

        // Act
        seeder.SeedInLine(mockCollection.Object);

        // Assert
        mockCollection.Verify(c => c.InsertBulk(It.IsAny<IEnumerable<QuestionDefinition>>()), Times.Once);
    }

    [Fact]
    public void SeedInLine_WithEmptyCollection_InsertsThreeQuestions()
    {
        // Arrange
        var mockDatabase = new Mock<ILiteDatabase>();
        var mockCollection = new Mock<ILiteCollection<QuestionDefinition>>();
        var mockEnv = new Mock<IWebHostEnvironment>();

        mockDatabase
            .Setup(db => db.GetCollection<QuestionDefinition>("question_definitions"))
            .Returns(mockCollection.Object);

        mockCollection.Setup(c => c.Count()).Returns(0);

        var insertedQuestions = new List<QuestionDefinition>();
        mockCollection
            .Setup(c => c.InsertBulk(It.IsAny<IEnumerable<QuestionDefinition>>(), It.IsAny<int>()))
            .Callback<IEnumerable<QuestionDefinition>, int>((questions, batchSize) => insertedQuestions.AddRange(questions))
            .Returns(3);

        var seeder = new DatabaseSeeder(mockDatabase.Object, mockEnv.Object);

        // Act
        seeder.SeedInLine(mockCollection.Object);

        // Assert
        insertedQuestions.Should().HaveCount(3);
    }

    [Fact]
    public void SeedInLine_GeneratedQuestionIds_AreUnique()
    {
        // Arrange
        var mockDatabase = new Mock<ILiteDatabase>();
        var mockCollection = new Mock<ILiteCollection<QuestionDefinition>>();
        var mockEnv = new Mock<IWebHostEnvironment>();

        mockDatabase
            .Setup(db => db.GetCollection<QuestionDefinition>("question_definitions"))
            .Returns(mockCollection.Object);

        mockCollection.Setup(c => c.Count()).Returns(0);

        var insertedQuestions = new List<QuestionDefinition>();
        mockCollection
            .Setup(c => c.InsertBulk(It.IsAny<IEnumerable<QuestionDefinition>>(), It.IsAny<int>()))
            .Callback<IEnumerable<QuestionDefinition>, int>((questions, batchSize) => insertedQuestions.AddRange(questions))
            .Returns(3);

        var seeder = new DatabaseSeeder(mockDatabase.Object, mockEnv.Object);

        // Act
        seeder.SeedInLine(mockCollection.Object);

        // Assert
        var ids = insertedQuestions.Select(q => q.Id).ToList();
        ids.Should().HaveCount(3);
        ids.Distinct().Should().HaveCount(3, "all IDs should be unique");
    }

    [Fact]
    public void SeedInLine_WithNonEmptyCollection_DoesNotInsertData()
    {
        // Arrange
        var mockDatabase = new Mock<ILiteDatabase>();
        var mockCollection = new Mock<ILiteCollection<QuestionDefinition>>();
        var mockEnv = new Mock<IWebHostEnvironment>();

        mockDatabase
            .Setup(db => db.GetCollection<QuestionDefinition>("question_definitions"))
            .Returns(mockCollection.Object);

        mockCollection.Setup(c => c.Count()).Returns(5);

        var seeder = new DatabaseSeeder(mockDatabase.Object, mockEnv.Object);

        // Act
        seeder.SeedInLine(mockCollection.Object);

        // Assert
        mockCollection.Verify(c => c.InsertBulk(It.IsAny<IEnumerable<QuestionDefinition>>()), Times.Never);
    }

    [Fact]
    public void SeedInLine_InsertsValidQuestionDefinitions()
    {
        // Arrange
        var mockDatabase = new Mock<ILiteDatabase>();
        var mockCollection = new Mock<ILiteCollection<QuestionDefinition>>();
        var mockEnv = new Mock<IWebHostEnvironment>();

        mockDatabase
            .Setup(db => db.GetCollection<QuestionDefinition>("question_definitions"))
            .Returns(mockCollection.Object);

        mockCollection.Setup(c => c.Count()).Returns(0);

        var insertedQuestions = new List<QuestionDefinition>();
        mockCollection
            .Setup(c => c.InsertBulk(It.IsAny<IEnumerable<QuestionDefinition>>(), It.IsAny<int>()))
            .Callback<IEnumerable<QuestionDefinition>, int>((questions, batchSize) => insertedQuestions.AddRange(questions))
            .Returns(3);

        var seeder = new DatabaseSeeder(mockDatabase.Object, mockEnv.Object);

        // Act
        seeder.SeedInLine(mockCollection.Object);

        // Assert
        insertedQuestions.Should().AllSatisfy(q =>
        {
            q.Id.Should().NotBeEmpty();
            q.Key.Should().NotBeNullOrEmpty();
            q.Label.Should().NotBeNullOrEmpty();
            q.Type.Should().NotBeNullOrEmpty();
            q.Required.Should().BeTrue();
            q.Placeholder.Should().NotBeNullOrEmpty();
        });

        // Verify specific questions
        var firstNameQuestion = insertedQuestions.First(q => q.Key == "first_name");
        firstNameQuestion.Label.Should().Be("First Name");
        firstNameQuestion.Type.Should().Be("text");

        var lastNameQuestion = insertedQuestions.First(q => q.Key == "last_name");
        lastNameQuestion.Label.Should().Be("Last Name");
        lastNameQuestion.Type.Should().Be("text");

        var emailQuestion = insertedQuestions.First(q => q.Key == "email");
        emailQuestion.Label.Should().Be("Email Address");
        emailQuestion.Type.Should().Be("email");
    }
}
