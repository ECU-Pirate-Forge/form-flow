using FluentAssertions;
using LiteDB;
using Moq;
using FormFlow.Backend;
using FormFlow.Data.Models;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using System.Linq;
using System.Text.Json;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace FormFlow.Backend.Tests;

public class DatabaseSeederTests
{
    [Fact]
    public void SeedFromJson_WithEmptyCollection_InsertsQuestionsFromJsonFile()
    {
        // Arrange
        var tempRoot = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(tempRoot);
        var seedDataDirectory = Path.Combine(tempRoot, "SeedData");
        Directory.CreateDirectory(seedDataDirectory);

        var seedFilePath = Path.Combine(seedDataDirectory, "questions.json");
        File.WriteAllText(seedFilePath, "[ { \"id\": \"00000000-0000-0000-0000-000000000000\", \"key\": \"first_name\", \"label\": \"First Name\", \"type\": \"text\", \"required\": true, \"placeholder\": \"Enter your first name\" } ]");

        var mockDatabase = new Mock<ILiteDatabase>();
        var mockCollection = new Mock<ILiteCollection<QuestionDefinition>>();
        var mockEnv = new Mock<IWebHostEnvironment>();

        mockEnv.SetupGet(e => e.ContentRootPath).Returns(tempRoot);
        mockCollection.Setup(c => c.Count()).Returns(0);
        var insertedQuestions = new List<QuestionDefinition>();
        mockCollection
            .Setup(c => c.InsertBulk(It.IsAny<IEnumerable<QuestionDefinition>>(), It.IsAny<int>()))
            .Callback<IEnumerable<QuestionDefinition>, int>((questions, batchSize) => insertedQuestions.AddRange(questions))
            .Returns(1);

        var seeder = new DatabaseSeeder(mockDatabase.Object, mockEnv.Object);

        // Act
        seeder.SeedFromJson(mockCollection.Object);

        // Assert
        mockCollection.Verify(c => c.InsertBulk(It.IsAny<IEnumerable<QuestionDefinition>>()), Times.Once);
        insertedQuestions.Should().HaveCount(1);
        insertedQuestions[0].Key.Should().Be("first_name");
        insertedQuestions[0].Label.Should().Be("First Name");
        insertedQuestions[0].Type.Should().Be("text");
        insertedQuestions[0].Id.Should().NotBe(Guid.Empty);
    }

    [Fact]
    public void SeedFromJson_WithNonEmptyCollection_DoesNotInsertData()
    {
        // Arrange
        var tempRoot = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(tempRoot);
        var seedDataDirectory = Path.Combine(tempRoot, "SeedData");
        Directory.CreateDirectory(seedDataDirectory);

        var seedFilePath = Path.Combine(seedDataDirectory, "questions.json");
        File.WriteAllText(seedFilePath, "[]");

        var mockDatabase = new Mock<ILiteDatabase>();
        var mockCollection = new Mock<ILiteCollection<QuestionDefinition>>();
        var mockEnv = new Mock<IWebHostEnvironment>();

        mockEnv.SetupGet(e => e.ContentRootPath).Returns(tempRoot);
        mockCollection.Setup(c => c.Count()).Returns(5);

        var seeder = new DatabaseSeeder(mockDatabase.Object, mockEnv.Object);

        // Act
        seeder.SeedFromJson(mockCollection.Object);

        // Assert
        mockCollection.Verify(c => c.InsertBulk(It.IsAny<IEnumerable<QuestionDefinition>>()), Times.Never);
    }

    [Fact]
    public void SeedFromJson_MissingSeedFile_ThrowsFileNotFoundException()
    {
        // Arrange
        var tempRoot = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(tempRoot);

        var mockDatabase = new Mock<ILiteDatabase>();
        var mockCollection = new Mock<ILiteCollection<QuestionDefinition>>();
        var mockEnv = new Mock<IWebHostEnvironment>();

        mockEnv.SetupGet(e => e.ContentRootPath).Returns(tempRoot);
        mockCollection.Setup(c => c.Count()).Returns(0);

        var seeder = new DatabaseSeeder(mockDatabase.Object, mockEnv.Object);

        // Act
        Action act = () => seeder.SeedFromJson(mockCollection.Object);

        // Assert
        act.Should().Throw<FileNotFoundException>();
    }

    [Fact]
    public void SampleJson_DeserializesIntoQuestionDefinition()
    {
        // Arrange
        var sampleJson = "[ { \"id\": \"00000000-0000-0000-0000-000000000000\", \"key\": \"first_name\", \"label\": \"First Name\", \"type\": \"text\", \"required\": true, \"placeholder\": \"Enter your first name\" } ]";

        // Act
        var questions = JsonSerializer.Deserialize<List<QuestionDefinition>>(sampleJson, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        // Assert
        questions.Should().NotBeNull().And.HaveCount(1);
        questions![0].Key.Should().Be("first_name");
        questions[0].Label.Should().Be("First Name");
        questions[0].Type.Should().Be("text");
        questions[0].Required.Should().BeTrue();
        questions[0].Placeholder.Should().Be("Enter your first name");
    }

    [Fact]
    public void SeedFromJson_InsertsExpectedNumberOfDocuments()
    {
        // Arrange
        var tempRoot = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(tempRoot);
        var seedDataDirectory = Path.Combine(tempRoot, "SeedData");
        Directory.CreateDirectory(seedDataDirectory);

        var seedFilePath = Path.Combine(seedDataDirectory, "questions.json");
        var json = "[ "
            + "{ \"id\": \"00000000-0000-0000-0000-000000000000\", \"key\": \"first_name\", \"label\": \"First Name\", \"type\": \"text\", \"required\": true, \"placeholder\": \"Enter your first name\" },"
            + "{ \"id\": \"00000000-0000-0000-0000-000000000000\", \"key\": \"last_name\", \"label\": \"Last Name\", \"type\": \"text\", \"required\": true, \"placeholder\": \"Enter your last name\" },"
            + "{ \"id\": \"00000000-0000-0000-0000-000000000000\", \"key\": \"email\", \"label\": \"Email Address\", \"type\": \"text\", \"required\": true, \"placeholder\": \"Enter your email address\" }"
            + " ]";
        File.WriteAllText(seedFilePath, json);

        var mockDatabase = new Mock<ILiteDatabase>();
        var mockCollection = new Mock<ILiteCollection<QuestionDefinition>>();
        var mockEnv = new Mock<IWebHostEnvironment>();

        mockEnv.SetupGet(e => e.ContentRootPath).Returns(tempRoot);
        mockCollection.Setup(c => c.Count()).Returns(0);

        var insertedQuestions = new List<QuestionDefinition>();
        mockCollection
            .Setup(c => c.InsertBulk(It.IsAny<IEnumerable<QuestionDefinition>>(), It.IsAny<int>()))
            .Callback<IEnumerable<QuestionDefinition>, int>((questions, batchSize) => insertedQuestions.AddRange(questions))
            .Returns(3);

        var seeder = new DatabaseSeeder(mockDatabase.Object, mockEnv.Object);

        // Act
        seeder.SeedFromJson(mockCollection.Object);

        // Assert
        mockCollection.Verify(c => c.InsertBulk(It.IsAny<IEnumerable<QuestionDefinition>>()), Times.Once);
        insertedQuestions.Should().HaveCount(3);
    }

    [Fact]
    public void SeedFromJson_IdsAreUniqueAndIndexed()
    {
        // Arrange
        using var memoryStream = new MemoryStream();
        using var database = new LiteDatabase(memoryStream);
        var collection = database.GetCollection<QuestionDefinition>("questions");

        var tempRoot = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(tempRoot);
        var seedDataDirectory = Path.Combine(tempRoot, "SeedData");
        Directory.CreateDirectory(seedDataDirectory);

        var seedFilePath = Path.Combine(seedDataDirectory, "questions.json");
        var json = "[ "
            + "{ \"id\": \"00000000-0000-0000-0000-000000000000\", \"key\": \"first_name\", \"label\": \"First Name\", \"type\": \"text\", \"required\": true },"
            + "{ \"id\": \"00000000-0000-0000-0000-000000000000\", \"key\": \"last_name\", \"label\": \"Last Name\", \"type\": \"text\", \"required\": true },"
            + "{ \"id\": \"00000000-0000-0000-0000-000000000000\", \"key\": \"email\", \"label\": \"Email Address\", \"type\": \"text\", \"required\": true }"
            + " ]";
        File.WriteAllText(seedFilePath, json);

        var mockDatabase = new Mock<ILiteDatabase>();
        var mockEnv = new Mock<IWebHostEnvironment>();

        mockEnv.SetupGet(e => e.ContentRootPath).Returns(tempRoot);

        var seeder = new DatabaseSeeder(mockDatabase.Object, mockEnv.Object);

        // Act
        seeder.SeedFromJson(collection);

        // Assert
        var questionIds = collection.FindAll().Select(q => q.Id).ToList();
        questionIds.Should().HaveCount(3);
        questionIds.Should().OnlyHaveUniqueItems();
        questionIds.Should().NotContain(Guid.Empty);
        var firstId = questionIds.First();
        var foundById = collection.FindById(firstId);
        foundById.Should().NotBeNull();
        foundById!.Key.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public void SeedFromJson_CollectionNotEmptyAfterSeeding()
    {
        // Arrange
        using var memoryStream = new MemoryStream();
        using var database = new LiteDatabase(memoryStream);
        var collection = database.GetCollection<QuestionDefinition>("questions");

        var tempRoot = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(tempRoot);
        var seedDataDirectory = Path.Combine(tempRoot, "SeedData");
        Directory.CreateDirectory(seedDataDirectory);

        var seedFilePath = Path.Combine(seedDataDirectory, "questions.json");
        File.WriteAllText(seedFilePath, "[ { \"id\": \"00000000-0000-0000-0000-000000000000\", \"key\": \"first_name\", \"label\": \"First Name\", \"type\": \"text\", \"required\": true } ]");

        var mockDatabase = new Mock<ILiteDatabase>();
        var mockEnv = new Mock<IWebHostEnvironment>();

        mockEnv.SetupGet(e => e.ContentRootPath).Returns(tempRoot);

        var seeder = new DatabaseSeeder(mockDatabase.Object, mockEnv.Object);

        // Act
        seeder.SeedFromJson(collection);

        // Assert
        collection.Count().Should().BeGreaterThan(0);
    }
}
